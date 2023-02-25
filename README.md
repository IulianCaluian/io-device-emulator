# io-device-emulator
This repository contains a software implementation of an input-output (IO) device emulator. 

## Description
It can emulate a device with actionable outputs, relays, and inputs that can be controlled through a web API. The emulator will simulate the behavior of a real-world device and provide a convenient way to test and validate system integrations without having to use physical hardware.

## Features
* Emulation of outputs and relays
* Web API for controlling outputs and relays
* Input simulation
* User interface for manual control

## Running the application
You can run multiple instances of the application on different ports to simulate multiple devices.
```
dotnet run --urls "http://192.168.1.100:5000"
```

## Barrier Simulation
The Barrier Simulation feature allows you to visualize how the activation of relays impacts a barrier, such as a gate or a door. This feature is designed to provide a visual representation of how the emulator is functioning and to demonstrate how the activation of a relay can trigger the movement of a barrier.

The simulation displays a graphical representation of the barrier and its components, including the relays and any other relevant components. You can use the user interface to manually activate and deactivate the relays, and the simulation will dynamically update to show the impact on the barrier. For example, when you activate a relay that is responsible for opening a gate, the simulation will show the gate opening and closing in response to the activation.

This feature is an interactive tool that provides a simple and intuitive way to test and validate the functionality of the emulator and to understand how the device it emulates would behave in a real-world scenario.
