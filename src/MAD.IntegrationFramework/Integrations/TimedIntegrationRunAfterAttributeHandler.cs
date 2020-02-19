using MAD.IntegrationFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationRunAfterAttributeHandler
    {
        public bool IsRunning(TimedIntegrationTimer check)
        {
            return !check.LastFinish.HasValue || check.LastStart > check.LastFinish;
        }

        public async Task WaitForOthers(TimedIntegrationTimer check, IEnumerable<TimedIntegrationTimer> toWaitFor)
        {
            RunAfterAttribute runAfterAttribute = check.TimedIntegrationType.GetCustomAttribute<RunAfterAttribute>();

            if (runAfterAttribute == null)
                return;

            TimedIntegrationTimer integrationTimerToRunAfter = toWaitFor.FirstOrDefault(y => y.TimedIntegrationType == runAfterAttribute.IntegrationTypeToRunAfter); ;

            if (integrationTimerToRunAfter == null)
                throw new MissingTimedIntegrationException(runAfterAttribute.IntegrationTypeToRunAfter);

            // If the interface to run after hasn't completed a first run, or it has started again but hasn't finished
            if (this.IsRunning(integrationTimerToRunAfter))
            {
                TaskCompletionSource<bool> waitForFinishTaskCompletionSource = new TaskCompletionSource<bool>();
                integrationTimerToRunAfter.PropertyChanged += InterfaceToRunAfterTimer_PropertyChanged;

                void InterfaceToRunAfterTimer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    // When the LastFinish property is updated, set the CompletionSource result so program flow may continue;
                    if (e.PropertyName == nameof(integrationTimerToRunAfter.LastFinish))
                    {
                        integrationTimerToRunAfter.PropertyChanged -= InterfaceToRunAfterTimer_PropertyChanged;
                        waitForFinishTaskCompletionSource.SetResult(true);
                    }

                }

                // Block the flow of the function until the LastFinish property has been updated
                await waitForFinishTaskCompletionSource.Task;
            }
        }
    }
}
