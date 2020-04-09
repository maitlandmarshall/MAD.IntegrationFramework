Repository
==========

This project is licensed under the MIT license.

MAD.IntegrationFramework
--------------------
MAD.IntegrationFramework is a .NET Standard 2.1 framework which exposes a simple unit of work pattern allowing you to quickly and easily create timed or scheduled "integrations". The framework automatically logs errors to a Microsoft SQL Server database and provides a robust and simple way to save, load and extend on configuration / meta data.


### Installation

For now, clone the repo and add it as a dependency to your .NET Core 3.1 project.

### Usage

First, you must bootstrap the framework by calling

```cs
MIF.Start();
```

Now the framework will pick up any classes which inherit from TimedIntegration automatically and start a timer based on its interval.

```cs
public class ExampleIntegration : TimedIntegration
{
	public override TimeSpan Interval => TimeSpan.FromMinutes(1);
	public override bool IsEnabled => true;

	public override Task Execute()
	{
		// Execute your unit of work here. 
		// Any errors will be automatically logged to the database if a SQL Connection String is provided in the configuration class.

		throw new NotImplementedException();
	}
}
```

### Configuration

The framework provides a base MIFConfig class for you to extend. Inherited properties will be automatically saved to a settings.json file next to the executable.

The base MIFConfig:
```cs
public class MIFConfig
{
	internal const int DefaultBindingPort = 666;

	public string SqlConnectionString { get; set; }

	public int BindingPort { get; set; } = DefaultBindingPort;
	public string BindingPath { get; set; }
}
```

An example of adding new savable properties:
```cs
public class ExampleMIFConfig : MIFConfig
{
	public string EmailAddress { get; set; }

	public ExampleMIFConfig()
	{
		this.SqlConnectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
	}
}
```

The MAD.IntegrationFramework exposes a web front-end bound to the BindingPort property. By default, you can save / load your MIFConfig meta data by browsing to http://localhost:666/configuration

![Screenshot of config page](https://github.com/maitlandmarshall/MAD.IntegrationFramework/raw/ReadmeUpdate_9-04-2020/wiki/configPage.png)

#### Using your MIFConfig

In order to access the settings in your MIFConfig, simply add it as a constructor parameter and the framework will automatically inject it for you.

```cs
public class ExampleIntegration : TimedIntegration
{
	private readonly ExampleMIFConfig config;

	public override TimeSpan Interval => TimeSpan.FromMinutes(1);
	public override bool IsEnabled => true;

	public ExampleIntegration(ExampleMIFConfig config)
	{
		this.config = config;
	}

	public override Task Execute()
	{
		// Execute your unit of work here. 
		// Any errors will be automatically logged to the database if a SQL Connection String is provided in the configuration class.

		throw new NotImplementedException();
	}
}
```

Contributing
==========

Go for it.
