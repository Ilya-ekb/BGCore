# Core Loop System Documentation

Welcome to the Core Loop System! 🎉 This documentation provides an overview of the architecture, classes, and how to use them effectively in your projects. 

## Table of Contents

1. [Overview](#overview)
2. [Getting Started](#getting-started)
3. [Key Components](#key-components)
   - [IDroppable](#idroppable)
   - [CoreLoop](#coreloop)
   - [Loopable](#loopable)
   - [ControlLoopable](#controlloopable)
   - [Timer](#timer)
   - [TimerFactory](#timerfactory)
   - [GEvent](#gevent)
5. [Usage](#usage)
6. [Events](#events)
7. [Examples](#examples)
8. [Contributing](#contributing)
9. [License](#license)

## Overview

The Core Loop System is designed for modular game development, allowing you to manage loops and timed events efficiently. The system provides an easy way to create loopable elements and handle timing events, making it ideal for game logic, animations, and timed actions.

## Getting Started

Install the solution as a submodule in your project:

```bash
git submodule add https://github.com/Ilya-ekb/BGCore.git [path/to/submodules/folder]
```

To start using the Core Loop System, simply include the relevant namespaces in your project files:

```csharp
using Core.Entities.Loopables;
using Core.LoopSystem;
using Core.Timers;
using Core.ObjectsSystem;
```

Make sure to initialize the loops system:

```csharp
Loops.Initiate();
```

## Key Components

### IDroppable

`IDroppable` is an interface that defines a contract for objects that can exist within the system and be dropped. Implementing this interface ensures that the class possesses the necessary capabilities for managing the object's state (alive or dead).

#### Main Methods and Properties:

- **bool IsAlive**: Indicates whether the object is alive.
- **event Action<IDroppable> Alived**: An event that is triggered when the object becomes alive.
- **event Action<IDroppable> Dropped**: An event that is triggered when the object is dropped.
- **void SetAlive()**: Sets the object to be alive.
- **void Drop()**: Drops the object from the system.

### BaseDroppable

`BaseDroppable` is an abstract class that implements the `IDroppable` interface. It provides basic functionality for working with objects that can be created and dropped within the system. The class manages the object's state and its events during transitions between states.

#### Main Methods and Properties:

- **string Name**: The name of the object based on its type.
- **bool IsAlive**: A protected property that indicates whether the object is alive.
- **event Action<IDroppable> Alived**: An event that is raised when the object is activated.
- **event Action<IDroppable> Dropped**: An event that is raised when the object is dropped.
- **IDroppable parent**: A reference to the parent object that is responsible for dropping the current object.

#### Main Methods:

- **void SetAlive()**: Marks the object as alive and raises the `Alived` event.
- **void Drop()**: Performs the drop operation on the object and raises the `Dropped` event.

#### Overridable Methods:

- **protected virtual void OnAlive()**: Called when the object is activated. Can be overridden to perform additional actions upon activation.
- **protected virtual void OnDrop()**: Called when the object is dropped. Can be overridden to perform additional actions before dropping.

### Example Usage

```csharp
public class MyDroppableObject : BaseDroppable
{
    public MyDroppableObject(IDroppable parent) : base(parent) { }

    protected override void OnAlive()
    {
        base.OnAlive();
        // Additional logic on aliving
        Console.WriteLine($"{Name} has been alive.");
    }

    protected override void OnDrop()
    {
        base.OnDrop();  
        // Additional logic before dropping
        Console.WriteLine($"{Name} has been dropped.");
    }
}

// Usage
var parentObject = new SomeMainDroppable(); // Example parent
var droppableObject = new MyDroppableObject(parentObject);
droppableObject.SetAlive(); // The object becomes alive
droppableObject.Drop(); // The object will be dropped and invoke additional logic
```


### CoreLoop

`CoreLoop` is the main class responsible for executing all loopable actions. It manages adding and removing loopables, as well as executing their respective actions each frame.

### Loopable

`Loopable` serves as the base class for any object that can be added to a loop. This class provides methods to register actions, define execution orders, and manage their lifecycle.

### ControlLoopable

`ControlLoopable` extends `Loopable` and implements `IControllable`, providing control over its activation, deactivation, pausing, and playing states. This class is useful for elements that require more intricate control.

### Timer

`Timer` is a specific implementation of `Loopable` designed for timing tasks. You can set a period, determine if it should play on awake, and define actions for when the timer reaches its period.

### TimerFactory

`TimerFactory` simplifies the creation and management of timers. It maintains a list of active timers, allowing you to stop, play, or pause all timers at once.

### GEvent

`GEvent` is an event system that allows for flexible event management across the application's lifecycle. It supports attaching, detaching, and invoking events using categories.

## Usage

To create a loopable element, derive from `Loopable` or `ControlLoopable`, and implement the desired functionality. Register your actions using the `LoopOn` method, and manage your object's lifecycle by calling `SetAlive` or `Drop`.

### Example Usage of a Timer in Unity Engine

```csharp
public class MyTimerUser : MonoBehaviour
{
    private ITimer myTimer;

    void Awake()
    {
        myTimer = TimerFactory.CreateTimer(Loops.Update, 1f, OnTimerReached, true);
    }

    private void OnTimerReached(ITimer timer)
    {
        Debug.Log("Timer reached: " + timer.Period);
    }

    void OnDestroy()
    {
        myTimer.Drop(); // Ensure to drop the timer when it’s no longer needed.
    }
}
```

## Events

You can use `GEvent` to manage events effectively. Here’s how to attach and invoke events:

```csharp
GEvent.Attach("MyEventCategory", MyEventHandler);
GEvent.Call("MyEventCategory", myData);
```

### Example Event Handler

```csharp
private void MyEventHandler(object[] data)
{
    Debug.Log("Event triggered with data: " + data[0]);
}
```

## Examples

Refer to the examples provided in the `Examples` folder within the source code for practical implementations of timers, loops, and events.

## Contributing

Feel free to contribute to the Core Loop System! Fork the repository, make changes, and submit a pull request. Any feedback or improvements are welcome to enhance user experience.
