# Muse.NET 
This library helps you connect to the Muse Headset. 
http://www.choosemuse.com/

The Muse broadcasts over Bluetooth LE (Low Energy).

To keep your application free of external dependencies, this library does not include external Muse drivers. 
It translates the messages comming from the Muse directly from the Bluetooth channel and translates them
into simple classes.

## Components
1. Muse.Net - the core library. Found as package on nuget.org as Muse.Net
2. Muse.Console - a few simple commands to test your device
3. Muse.LiveFeed - a WinForms GUI that shows you a live graph of the EEG wave data feeds.

## Platform
These are dotnet core SDK projects, targeting net framework.  

### WinRT bluetooth API's 
The Muse.Net library uses the Bluetooth APIs from WinRT (Uwp)
The DLL's included in the source code and copied to the output folder.
These libraries are not cross platform / netcoreapp compatible. 

These are included in the package, so there is no need to add them manually. However they are normally found here:
- C:\Program Files (x86)\Windows Kits\10\UnionMetadata\Windows.winmd
- C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll

## Background

### GATT
This library is built on top of the Gatt (Generic Attributes) interface for Bluetooth.
https://www.bluetooth.com/specifications/gatt/generic-attributes-overview

### Device output
THe Muse Headset has several sensors:
- Accelerometer (when the devices is moving)
- Gyroscope (when the device is turning)
- Telemetry (battery, temperature)
- Brain waves (2 front, 2 rear + Auxilary)

### Communication
The device broadcasts each sensor output over a separate Gatt channel (characteristic). 
Broadcasts only occur over channels to which your app is subscribed.

## Usage
```csharp

	var client = new MuseClient(MyMuse.Address);
	var ok = await client.Connect();
	if (ok)
	{
		await client.Subscribe(
			Channel.EEG_AF7, 
			Channel.EEG_AF8, 
			Channel.EEG_TP10, 
			Channel.EEG_TP9,
			Channel.EEG_AUX);

		client.NotifyEeg += Client_NotifyEeg;
		await client.Resume();
	}
```

## Subscribable events
The MuseClient has four events you can subscribe to: NotifyTelemetry, NotifyAccelerometer, NotifyGyroscope, NotifyEeg.
Telemetry shows you battery, voltage and temperature. All the 5 EEG channels are broadcasted over the NotifyEeg event.


## Attribution
Thanks to Carter Appleton for pointing a way to access WinRT.
[Win10Win32Bluetooth](https://github.com/CarterAppleton/Win10Win32Bluetooth)

A lot of the technology in this library has been based on [muse-js](https://github.com/urish/muse-js)