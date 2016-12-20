# AppKit.UI
App accelerator pack to bring common UI functionality easy with Xamarin

## What is AppKit?
**AppKit.UI** is a cross-platform PCL library built over Xamarin and its purpose is to make developer life easier. Under the hood it does nothing _special_, it consumes standard **Xamarin.iOS** & **Xamarin.Android** libraries, plus it extends some basic UI concepts of the **iOS UI API** and the **Anroid UI API**. The fun part is that it facilitates for you UI specific tasks like: passing data between controllers and fragments, lists binding, lists events handling, gesture handling, and much more. In short, it simply adds some compensation between the differences of the **iOS** platform and the **Android** platform. It joins the "best" of the two platforms to bring a _common_ way to design and code UI logic in your app.

## What is not AppKit.UI?
AppKit is not a "write-once" library like **Xamarin.Forms**. It not brings a unique programming model which allows you to write UI views and UI logic once for each platform you want to target. You still need to create separate UI views (.xib or .axml) and UI code logic to wire up widgets events. 

## Do I need it?
No, if you like to write boring code from the very base, countless times in any app you develop. 
Yes, if you want to speed up the development of your apps, leaving **AppKit.UI** to do UI tasks for you.

## How it helps me?
**AppKit.UI** helps you in a very simple way. It allows you to develop in a _happy_ and very _easy-porting_ mode. You still have full control over the _native platform_ you want to target, with the only exception that you will have at you disposal many enanched or helper classes that will _unify_ UI logic.

Steps involved to do this are the following:
 1. First you create a Cross-Platform -> Blank App (Native Portable) 
 2. Add the **AppKit** Nuget package to your solution (optional but enache this experience)
 3. Add the **AppKit.UI** Nuget package to your solution
 4. Then choose an "initial" platform (could be **iOS** or **Android**, it doesn't matter) and start to design and create your UI views
 6. When the "bones" are ready you wire up widgets events to gather input or execute actions (**AppKit.UI** will help you here)
 7. Then you _port_ the app to the other platform, but a this point you just need to create a "copy" of the _original_ UI you designed and wire up events (**AppKit.UI** will help you here)

## An important thing to note!
Developing cross-platform apps with **AppKit.UI** leads to one major limitation. You can't use any specific UI control or UI feature that is not "abstractable" by the library itself. At least, you can, but you will lose the _easy-porting_ mode. So for example, you can't use a **Storyboard** to design your UI in **iOS**, as a matter of fact, there's nothign similar in **Android**.

Think about **AppKit.UI** as a _facilitator_ than a real tool to develop Xamarin apps. **Remember**, if you need a real tool to create cross-platform apps, you should consider using other stuff like **Xamarin.Forms** or **MvvmCross**.

## Ok, I want to try it!
Let's start with some basic stuff. 

Remember that **AppKit.UI** is more an _accelerator_ than a tool, so keep in mind that this is **NOT** the _only_ way to create apps with **Xamarin**.

After you create your _Cross-Platform -> Blank App (Native Portable)_ solution add the **AppKit.UI** Nuget package to all projects.

Then you are ready to begin!

### The App Structure
First of all we propose a "structure model" for your app. 

A good strategy colud be to "divide" your app in two macro blocks: _core logic_ and _UI logic_. The first serves as the business logic of your app, while the second incapsulates all UI implementations.

This comes naturally due to the fact that your _Blank App (Native Portable)_ solution is already organized like this.
In your newly created solution you will find:
- A _PCL_ project which is ideal to use as a central core logic implementation
- A **Xamarin.iOS** app project which is where you create the UI logic for **iOS**
- A **Xamarin.Android** app project which is where you create the UI logic for **Android**

##### Core Logic:
_This logic is implemented in the **PCL project** of your solution_
```           
 AppController // static class you need to create which holds all the common business logic of your app
      |
      |
      +--FunnyAction1() // static method which do logic stuff. appkit will come handy here!
      +--FunnyAction2()
      +--FunnyAction3()
      ...
      +--AnotherFunnyAction()
```

##### UI Logic:
_All these elements are created in the **platform specific projects** of your solution_
```
 AppEntryPoint: class you need to implement
      |           
      |
      +-MainAppScreen: this is usually a UINavigationViewController class or an Activity class
               |
               |
               +-SubAppScreen1: this is usually a UIViewController class or a Fragment class
               +-SubAppScreen2  here you consume your AppController.Action methods
               +-SubAppScreen3
               ...
               +-SubAppScreenN    
```

### Core Logic: The AppController
An _AppController_ is nothing more than a static class full of static useful methods which represent the business core logic of your app. Usually these methods do things like: write to a database, call a rest service, write/read files to/from the internal or external memory. 

To accomplish this you should use specific platform API or integrate other external PCL libraries. 

You are free to create your _AppController_ class from the very base **OR** you can have a look to [**AppKit**](https://github.com/admaiorastudio/appkit) which is another _acceleration tool_ which could be handy for this task.

If you want to know more about **AppKit**, check this out!
https://github.com/admaiorastudio/appkit

### UI Logic: The MainAppScreen  
A **MainAppScreen** usually represents your main UI _holder_ which will contains all others **SubAppScreen** elements. 
This object will be responsible for few major things in your app, one of the most important is how you will _navigate_ between the various _sections_ of your app. Each _section_ can be represented by a single **SubAppScreen** or a set of them, one of which will represent the "root" of the _section_.

```
 +-MainAppScreen
       |
       |
       +-(section1) SubAppScreen1
       |
       +-(section2) SubAppScreen2A->SubAppScreen2B->SubAppScreen2C
```

Navigation can be handled in several ways in your app. Each platform gives you different UI controls to achieve this but sometimes some of these controls are not "portable", from one platform to another. 

Generally speaking this is not a problem, if you want to give the user a _separated platform_ experience. 

_Au contraire_, if you are planning to give the user an _unieque experience_, you have to face the fact that there's only a way to do this. You have to use UI controls and UI features that can be easly ported from one platform to another.

**Don't despair!** This is not necessarily bad as it seems. A KISS approach could be a _win win_ choice in most cases.

Let's see how **AppKit.UI** can help us on this quest! 

The idea is to exploit the parallelism between the **UIViewController** class with the support of a **UINavigationViewController** class in **iOS** and the **Activity** class with the support of a **FragmentManager** class in **Android**.

In iOS **AppKit.UI** will expose the **AdMaiora.AppKit.UI.App.UIMainViewController** class, which is an implementation of the **UIViewController** class that internally contains a **UINavigationController** class instance. 

In Android **AppKit.UI** will expose the **AdMaiora.AppKit.UI.App.AppCompactActivity** class, which is an implementation of the **Android.Support.V7.App.AppCompatActivity** class that internally contains a **SupportFragmentManager** class instance. 

These class implementations are created to bring new methods and funtionalities which will fill the gap between the two platforms. We will see them in action later!

Your **MainAppScreen** element in **iOS** will be like this ([download here the related .xib file](https://dl.dropboxusercontent.com/u/40411013/admaiora/appkit/resources/Layouts/MainViewController.xib)):

```cs
    public partial class MainViewController : AdMaiora.AppKit.UI.App.UIMainViewController
    {
        #region Constructors

        public MainViewController()
            : base("MainViewController", null)
        {
        }

        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            #region Designer Stuff
            
            SetContentView(this.ContentLayout);
            
            #endregion
            
            this.LoadLayout.UserInteractionEnabled = true;
            this.LoadLayout.Hidden = true;
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Block the main UI, preventing user from tapping any view
        /// </summary>
        public void BlockUI()
        {
            if (this.LoadLayout != null)
                this.LoadLayout.Hidden = false;
        }

        /// <summary>
        /// Unblock the main UI, allowing user tapping views
        /// </summary>
        public void UnblockUI()
        {
            if (this.LoadLayout != null)
                this.LoadLayout.Hidden = true;
        }

        #endregion
    }
```

Your **MainAppScreen** element in **Android** will be like this ([download here the related .axml file](https://dl.dropboxusercontent.com/u/40411013/admaiora/appkit/resources/Layouts/ActivityMain.axml)):

```cs
    [Activity(
        Label = "YourAppNameHere",
        ScreenOrientation = ScreenOrientation.Portrait,
        LaunchMode = LaunchMode.SingleTask,
        ConfigurationChanges =
            ConfigChanges.Orientation | ConfigChanges.ScreenSize |
            ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard
    )]
    public class MainActivity : AdMaiora.AppKit.UI.App.AppCompactActivity
    {
        #region Constructors

        public MainActivity()
        {
        }

        #endregion
        
        #region Activity Methods

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            #region Desinger Stuff

            SetContentView(Resource.Layout.ActivityMain, Resource.Id.ContentLayout);
            
            this.SupportActionBar.SetDisplayShowHomeEnabled(true);
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);             
            
            #endregion
            
            this.LoadLayout.Focusable = true;
            this.LoadLayout.FocusableInTouchMode = true;
            this.LoadLayout.Clickable = true;
            this.LoadLayout.Visibility = ViewStates.Gone;            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Block the main UI, preventing user from tapping any view
        /// </summary>
        public void BlockUI()
        {
            if (this.LoadLayout != null)
                this.LoadLayout.Visibility = ViewStates.Visible;
        }

        /// <summary>
        /// Unblock the main UI, allowing user tapping views
        /// </summary>
        public void UnblockUI()
        {
            if (this.LoadLayout != null)
                this.LoadLayout.Visibility = ViewStates.Gone;
        }

        #endregion         
    }
```

### UI Logic: The SubAppScreen  
A **SubAppScreen** usually represent a single section of your app or a part of it. Each **SubAppScreen** will contain one or more UI widgets and UI logic to let the user do some stuff. Remember to put minimal code in the UI logic and let the _AppController_ class do the heavy stuff, this will be useful when porting the app from one platform to another.

Let’s see how **AppKit.UI** can help us on this quest!

The idea is to exploit the parallelism between the **UIViewController** class in **iOS** and the **Fragment** class in **Android**.

In iOS **AppKit.UI** will expose the **AdMaiora.AppKit.UI.App.UISubViewControllerr** class, which is an implementation of the **UIViewController** class. 

In Android **AppKit.UI** will expose the **AdMaiora.AppKit.UI.App.Fragment** class, which is an implementation of the **Android.Support.V4.App.Fragment** class. 

These class implementations are created to bring new methods and funtionalities which will fill the gap between the two platforms. We will see them in action later!

Your **SubAppScreen** element in **iOS** will be like this:

```cs
    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        #region Constructors

        public SubViewController()
            : base("SubViewController", null)
        {
        }

        #endregion
        
        #region Widgets
        
        [Widget]
        private FrameLayout LoadLayout;

        #endregion

        #region Properties
        #endregion

        #region ViewController Methods

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff
            #endregion
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        #endregion
    }
```

Your **SubAppScreen** element in **Android** will be like this:

```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Constructors

        public SubFragment()
        {
        }

        #endregion

        #region Fragment Methods

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentSub, inflater, container);            

            #endregion     
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        #endregion    
    }
```

### UI Logic: Compensation
Till now, we didn't see much _enachements_ in action. This because we had to prepare the "foundation" where you can start using the various UI _compensation_ features and widgets included in **AppKit.UI**. Actually, you have already begun but maybe you didn't noticed it!

Take a look to this piece of code in the **Android** implementation of your **SubAppScreen** element implementation:
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff

            SetContentView(Resource.Layout.FragmentSub, inflater, container);            

            #endregion     
        }    
    }
```

As you can see this is **NOT** a classic _OnCreateView()_ method override for a standard **Fragment** class!

Let's see it more in detail in the next sections!

#### Widgets
___
Widgets are also known as UI controls, each platform has an API for the UI which expose several of these controls. They could have different names but in general you know them as: label, textbox, button, list or wathever. When you design and create an app section (**SubAppScreen**) you put a lot of this stuff inside it. Half job. Next you have to wire up widgets events or gather/put values from/to them. This is what we call UI logic. To accomplish this in a _cross-platform_ way you need to get a "refernce" of these widgets instances and then hook events or read/set properties.

##### The iOS Way
In **iOS** this is quite simple, for each widget you need to interact with you must create an _Outlet_. Doing this and thanks to Xamarin, the _Outlet_ is autoamtically translated as a _private member_ in your **ViewControllerClass** class implementation. Easy, nothing more to say about this.

##### The Android Way
In **Android** there is no _Outlet_ creation, for each widget you need to interact with you must first assign an _id_ to it. Later, in your **Fragment** code, you have to manually define a _private member_ which will be used to hold the result of the _FindViewById()_ method. This is frustrating, especially when you have many widgets. 

**AppKit.UI** comes in your help here. How? Well, you still need to assign an _id_ to the widget and yes, in your **Fragment** code, you still have to define a _private member_, but this time you decorate it with the **[Widget]** attribute. Easy! This magic occours only if your widget _id_ and your private member share the same naming.

Let's see an example.

In your _.axml_ define a button like this:
```xml
    <Button
        android:id="@+id/LoginButton"
        android:text="LOGIN"
        android:layout_width="300dp"
        android:layout_height="40dp" />
```

Add this code to your fragment:
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        ...
        
        #region Widgets
        
        [Widget]
        private Button LoginButton;
        
        #endregion
        
        ...
    }
```

Then override this method:
```cs
    public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
    {
        base.OnCreateView(inflater, container);

        #region Desinger Stuff

        SetContentView(Resource.Layout.FragmentSub, inflater, container);            

        #endregion    
        
        this.LoginButton.Click += (s, e) =>
            {
            };
    }
```

Well done! All the magic is done via the new shining _SetContentView()_ method, once called it will gather for you all the widgets marked with the **[Widget]** attribute.

#### Resize and Panning
___
One of the most annoing problem that afflict _app design_ is the "keyboard overlay issue". Depending on your needing, you might find yourself in the situation of having to create an app screen that contains one or more textboxes which will be filled by the user via the _keyboard input_ method. When the user start to enter data, the _soft keyboard_ will appear on the screen, hiding much of what the user can see and interact with. This is annoying, especially in **iOS** where no "back button" is available to hide the keyboard.

To solve this issue you may choice different solutions. Two of them are the most used: to _scroll_ the app screen when a certain textbox is focused (also known as "panning) or to _resize" the app screen when a certain textbox is focused.

##### The iOS Way
In **iOS** there is nothing precooked for you. You have to manually handle panning (maybe using the UIScrollView) or keyboard events to resize your app screen.

**AppKit.UI** comes in your help here exposing two methods: the _SlideUpToShowKeyboard()_ method and the _ResizeToShowKeyboard()_ method.

They are relative easy to use, just see the example below:
```cs
    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff
            
            // This will enable automatic "scrolling" in your app screen
            SlideUpToShowKeyboard();
            
            #endregion            
        }
    }
```
```cs
    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff
            
            // This will enable automatic "resizing" in your app screen
            ResizeToShowKeyboard();
            
            #endregion            
        }
    }
```

**AppKit.UI** helps you more here, exposing three new metods: the _AutoShouldReturnTextFields()_ method, the _AutoDismissTextFields()_ method, the _DismissKeyboard()_ method and the _RequestUserInput()_ extension method for the **UITextField** widget.

The _AutoShouldReturnTextFields()_ method is used to let the user automatically "jump" from an UITextField to another UITextFiled when the _return button_ is pressed. This facilitate the filling of a typical form input app screen.

The _AutoShouldReturnTextFields()_ method allows the user to automatically dismiss the keyboard when he taps outside the focused **UITextField** widget.

The _DismissKeyboard()_ method allows you to programmatically dismiss the keyboard. This is useful to notify the user that the "input flow" of your form input app screen is ended.

The _RequestUserInput()_ extension method for the **UITextField** widget allows you to issue an "auto focus" of the textbox, forcing the keyboard to appear on the screen.

They are relative easy to use, just see the example below:
```cs
    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff
            
            // This will enable automatic "jump" between the two UITextField widgets
            AutoShouldReturnTextFields(new[] { this.EmailText, this.PasswordText });

            // This will enable automatic "keyboard dismiss" when the user taps outside the two UITextField widgets
            AutoShouldReturnTextFields(new[] { this.EmailText, this.PasswordText });

            #endregion    
            
            this.LoginButton.TouchUpInside += LoginButton_TouchUpInside;
        }
        
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
    
            // This will force the keyboard to appear on the screen right after the SubViewController is presented
            this.InputText.RequestUserInput();
        }        
        
        private void LoginButton_TouchUpInside(object sender, EventArgs e)
        {
            LoginUser();
            
            // This will dismiss the keyboard right after the user press the login button
            DismissKeyboard();
        }        
    }
```

##### The Android Way
In **Android** you have some native functionalities to handle this, some of them are easy to use, some others are not!

To keep the _easy-porting_ spirit **AppKit.UI** exposes to you a set of similar methods: the _SlideUpToShowKeyboard()_ method and the _ResizeToShowKeyboard()_ method.

They are relative easy to use, just see the example below:
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff
            
            SetContentView(Resource.Layout.FragmentSub, inflater, container);
            
            // This will enable automatic "scrolling" in your app screen
            SlideUpToShowKeyboard();

            #endregion
        }
    }
```
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff
            
            SetContentView(Resource.Layout.FragmentSub, inflater, container);
            
            // This will enable automatic "resizing" in your app screen
            SlideUpToShowKeyboard();

            #endregion
        }
    }
```

**AppKit.UI** helps you more here, exposing two metods: the _DismissKeyboard()_ method  and the _RequestUserInput()_ method extension for the **EditText** widget.

_(You may notice that the two methods: _AutoShouldReturnTextFields()_ and _AutoShouldReturnTextFields()_ are missing in the **Android** implementation of the **AppKit.UI**. Don't worry! You don't need them, that is because **Android** handles natively this aspect)_

The _DismissKeyboard()_ method allows you to programmatically dismiss the keyboard. This is useful to notify the user that the "input flow" of your form input app screen is ended.

The _RequestUserInput()_ extension method for the **EditText** widget allows you to issue an "auto focus" of the textbox, forcing the keyboard to appear on the screen.

They are relative easy to use, just see the example below:
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff
            
            SetContentView(Resource.Layout.FragmentSub, inflater, container);
            
            #endregion
            
            this.LoginButton.Click += LoginButton_Click;
        }
        
        public override void OnStart()
        {
            base.OnStart();

            // This will force the keyboard to appear on the screen right after the SubFragment is presented
            this.InputText.RequestUserInput();
        }
        
        private void LoginButton_Click(object sender, EventArgs e)
        {
            LoginUser();

            // This will dismiss the keyboard right after the user press the login button
            DismissKeyboard();            
        }        
    }
```

#### Keyboard Events
___
Desiging your app screen, it could happen that you might find in the need to know the visibility state of the _soft keyboard_. One example is when you want to make some sort of animation when the user focuses a certain textbox in your app screen.

Achieving this is not so easy, especially in the **Android** platform.

##### The iOS Way
Using an _observer_ for the _UIKeyboardWillChangeFrameNotification_ message is the preferred way, but you need to write a lot of code for this.

**AppKit.UI** comes in your help here exposing two new UIViewController lifecycle methods: the _KeyboardWillShow()_ method and the _KeyboardWillHide()_ method. To activate/deactivate the notification of these two methods you need to use: the _StartNotifyKeyboardStatus()_ method and the _StopNotifyKeyboardStatus()_ method.

They are relative easy to use, just see the example below:
```cs
    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            #region Designer Stuff
            #endregion   
            
            // This will enable notifications of the keyboard visibility status
            StartNotifyKeyboardStatus();
        }
        
        public override void KeyboardWillShow()
        {
            base.KeyboardWillShow();
            // Add here logic to be executed when the keyboard appears
        }
        
        public override void KeyboardWillHide()
        {
            base.KeyboardWillHide();
            // Add here logic to be executed when the keyboard disappears
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            
            // This will stop notifications of the keyboard visibility status
            StopNotifyKeyboardStatus();
        }
    }
```

##### The Android Way
No _observers_ in **Android**, you have different techniques available to achieve this,  but again you need to write a lot of code for this.

**AppKit.UI** comes in your help here exposing two new **Fragment** lifecycle methods: the _OnKeyboardShow()_ method and the _OnKeyboardHide()_ method. To activate/deactivate the notification of these two methods you need to use: the _StartNotifyKeyboardStatus()_ method and the _StopNotifyKeyboardStatus()_ method.

They are relative easy to use, just see the example below:
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);

            #region Desinger Stuff
            
            SetContentView(Resource.Layout.FragmentSub, inflater, container);
            
            #endregion
            
            // This will enable notifications of the keyboard visibility status
            StartNotifyKeyboardStatus();            
        }
        
        public override void OnKeyboardShow()
        {
            base.OnKeyboardShow();
            // Add here logic to be executed when the keyboard appears
        }
        
        public override void OnKeyboardHide()
        {
            base.OnKeyboardHide();
            // Add here logic to be executed when the keyboard disappears
        }
        
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            
            StopNotifyKeyboardStatus();
        }
    }
```

#### Top Bar Actions
___
The _easiest_ and _cleanest_ way to let the user do some contextual stuff (actions) in your **SubAppScreen** is to add _top bar action_ items. These are named and handled differently in **iOS** and **Android** but usually appear on the top of your screen, in the right side of the navigation bar. 

##### The Android iOS
To add a _top bar action_ you usually need to create a **UIBarButtonItem** and assign it to the _RightBarButtonItem_ property of the _NavigationItem_ in your UIViewController. Certainly not "rocket sience" but it forces you to write a lot of code.

**AppKit.UI** comes in your help here exposing two new **UIViewController* lifecycle methods: the _CreateBarButtonItems_ method and the _BarButtonItemSelected()_ method. These mehtods are modeled on the **Android** _options menu_ handling to keep the _easy-port_ spirit alive!

They are relative easy to use, just see the example below:
```cs

    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        public override bool CreateBarButtonItems(UIBarButtonCreator items)
        {
            base.CreateBarButtonItems(items);

            // Adds here as many "top actions" you need
            // Items added are indexed starting from zero
            items.AddItem("Action1", UIBarButtonItemStyle.Plain);
            
            // Return true if you want to create them in an animated way
            return true;
        }

        public override bool BarButtonItemSelected(int index)
        {
            // Handle here your "top actions" created
            switch (index)
            {
                case AppKit.UI.App.UISubViewController.BarButtonBack:
                    
                    // This is a special case to handle the "back" button
                    // Write here any custom logic to prevent "back navigation"
                    return true;
            
                case 0:
                    
                    // Do your custom logic for the "Action1" top action
                    // Return true to notify a custom handling of the pressed top action
                    return true;

                default:
                    return base.BarButtonItemSelected(index);
            }
        }    
    }
```

##### The Android Way
There's nothing special to say here, you still use what **Android** has ready for you. 

```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            
            // Clear any previous "top actions" created
            menu.Clear();
            
            // Adds here as many "top actions" you need
            // Items added are indexed starting from zero            
            menu.Add(0, 0, 0, "Action1").SetShowAsAction(ShowAsAction.Always);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch(item.ItemId)
            {
                case Android.Resource.Id.Home:
                
                    // This is a special case to handle the "back" button
                    // Write here any custom logic to prevent "back navigation"                
                    return true;
                    
                case 0:
                    
                    // Do your custom logic for the "Action1" top action
                    // Return true to notify a custom handling of the pressed top action
                    return true;                    

                default:
                    return base.OnOptionsItemSelected(item);
            }            
        }    
    }
```

#### Data Propagation
___
As we said before, your app is designed to contains one or more _sections_. Each section is represented by a single or multiple "SubAppScreen" elements. In a "multiple screen" scenario, you might find yourself in the need to pass data between these screens. 

##### The Android iOS
In **iOS** there is nothing precooked for you. You have to manually handle data propagation when switching from an **UIViewController** to another. To achieve this you usually need to expose one ore more public properties in your "destination" **UIViewController** and popuplate them in your "origin" **UIViewController**. Alternatively you could parametrize the "destination" **UiViewController**. Another way is to forwarding data using _segues_, but this could not be a good choice to keep the _easy-porting_ mode valid.

**AppKit.UI** comes in your help here with the "bundle" concept. Each **UIViewController** is equiped now with a _Arguments_ property which can be popuplated with a new  **UIBundle** object instance. This object works as a "property bag" (dictionary) where you can store one or more value. Each value is added via an _identification key_. The ** UIBundle** can store primitives types like: integers, strings, boolean, datetime but also support basic JSON serialization, that is, to allow you to store complex object. This is very useful when you need to pass several values that belong to the same context.

It's relative easy to use, just see the example below:
```cs
    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        private void MagicButton_Click(object sender, EventArgs e)
        {
            // This will create a new instance of the AnotherSubViewController
            // then populate its Arguments property with a new UIBundle instance filled
            // with some values. You can store as many value you need
            // Finally we push the AnoterSubViewController into the navigation controller
            var c = new AnotherSubViewController();
            c.Arguments = new UIBundle();
            c.Arguments.PutInt("AnIntValue", 10);
            c.Arguments.PutString("AStringValue", "hello world!");
            this.NavigationController.PushViewController(c, true);
        }
    }
```

In our "destination" **UIViewController** we gather the forwarded values like this:
```cs
    public partial class AnotherSubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Here we gather the "AnIntValue" forwarded value
            int anIntValue = this.Arguments.GetInt("AnIntValue");
            // Like above we gather here the "AStringValue" forwarded value
            int aStringValue = this.Arguments.GetString("AStringValue");
        }    
    }
```

##### The Android Way
Again, there’s nothing special to say here, you still use what Android has ready for you. You can "bundle" it all the time!

```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        private void LoginButton_Click(object sender, EventArgs e)
        {
            var f = new AnotherSubFragment();
            f.Arguments = new Bundle();
            f.Arguments.PutInt("AnIntValue", 10);
            f.Arguments.PutString("AStringValue", "hello world!");
            this.FragmentManager.BeginTransaction()
                .AddToBackStack("BeforeAnotherSubFragment")
                .Replace(Resource.Id.ContentLayout, f, "AnotherSubFragment")
                .Commit();        
        }
    }
```

In our "destination" **Fragment** we gather the forwarded values like this:
```cs
    public class AnotherSubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Here we gather the "AnIntValue" forwarded value
            int anIntValue = this.Arguments.GetInt("AnIntValue");
            // Like above we gather here the "AStringValue" forwarded value
            int aStringValue = this.Arguments.GetString("AStringValue");            
        }
    }
```

#### Lists
___
Lists are one of the most common Widget used in your UI compositions. They are very useful when you need to show a scrollable group of items, allowing the user to select one or more elements from it. Either **iOS** and **Android** support you with a complete API to get this. The way you implement it in each platform is quite similar, but you have to write a lot of code, that usually can be reused and simplified.

##### The iOS Way
In **iOS** you have the **UITableView** widget and the **UICollectionView** widget. Implementing them is not that hard, but a lot of code is required to make them work, moreover concepts like cell reusability or item selection events needs to be implemented constantly in your code.

**AppKit.UI** comes in your help here exposing two new widgets: the **UIItemListView** widget and the **UIItemCollectionView** widget. _De facto_ these two new widgets are an "extension" of what you already have. Their only purpose is to simplify and reduce the code you need to write in your UI logic, moreover these controls expose some additional events that simplify item selection.

To simplify data binding, **AppKit.UI** brings to you two new _data objects_ to be used as a source for the **UIItemListView** widget and the **UIItemCollectionView** widget, they are: the **UIItemListViewSource<T>** class and the **UIItemCollectionViewSource<T>** class. In the same way these two _data objects_ are an "extension" of what you already have to populate your lists. Their only purpose is to simplify and reduce the code you need to write in your UI logic.

Let's see some code in action:
```cs

    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        #region Inner Classes
        
        // We define a simple "model" object
        private class DataItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }
        
        // We define our "data object" to bind our list
        // As you can see the UIItemListViewSource is a generic type class
        private class DataViewSource : UIItemListViewSource<DataItem>
        {      
            // We need expose a public constructor which basically set some base informations
            // like what UIViewController will host this data source, the name of the .xib that will
            // be used to create all the UITableViewCell instances (with reusability) and the data source 
            public DataViewSource(UIViewController controller, IEnumerable<DataItem> source)
                : base(controller, "DataItemViewCell", source)
            {
            }    
        
            // We need to override this method to get the instance of the UITableViewCell that will be rendered
            // and to bind data to its content
            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath, UITableViewCell cellView, DataItem item)
            {
                // This cast the base cell type to our specific implementation
                var cell = cellView as DataItemViewCell;
                
                // Do any binding o UI logic here
                
                return cell;
            }
        }
        
        #endregion
        
        #region ViewController Methods
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            var source = new DataItemViewSource(this, new DataItems[]
            {
                new DataItem { Title = "Title1", Description = "Lore ipsum dolor sit" },
                new DataItem { Title = "Title2", Description = "Lore ipsum dolor sit" },
                new DataItem { Title = "Title3", Description = "Lore ipsum dolor sit" },
                new DataItem { Title = "Title4", Description = "Lore ipsum dolor sit" }
            });
            
            this.DataItemList.Source = _source;
            this.DataItemList.RowHeight = UITableView.AutomaticDimension;
            this.DataItemList.EstimatedRowHeight = 88;
            this.DataItemList.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.DataItemList.BackgroundColor = ViewBuilder.ColorFromARGB(AppController.Colors.DarkLiver);
            this.DataItemList.TableFooterView = new UIView(CoreGraphics.CGRect.Empty);
            this.DataItemList.ItemSelected += DataItemList_ItemSelected;
        }   
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            
            this.DataItemList.ItemSelected -= DataItemList_ItemSelected;
        }
        
        #region ViewController Methods
        
        #region Event Handlers
        
        private void DataItemList_ItemSelected(object sender, UIItemListSelectEventArgs e)
        {
            DataItem item = e.Item as DataItem;
            // Do whatever you need with the selected DataItem
        }        
        
        #endregion
    }
```

##### The Android Way
In **Android** you have the **ListView** widget and the **RecyclerView** widget. Implementing them is not that hard, but a lot of code is required to make them work, moreover concepts like cell reusability or item selection events needs to be implemented constantly in your code.

**AppKit.UI** comes in your help here exposing two new widgets: the **ItemListView** widget and the **ItemRecyclerView** widget. _De facto_ these two new widgets are an "extension" of what you already have. Their only purpose is to simplify and reduce the code you need to write in your UI logic, moreover these controls expose some additional events that simplify item selection.

To simplify data binding, **AppKit.UI** brings to you two new _data objects_ to be used as a source for the **ItemListView** widget and the **ItemRecyclerView** widget, they are: the **ItemListAdapter<T>** class and the **ItemRecyclerAdapter<T>** class. In the same way these two _data objects_ are an "extension" of what you already have to populate your lists. Their only purpose is to simplify and reduce the code you need to write in your UI logic.

Let's see some code in action:
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        #region Inner Classes
        
        // We define a sime "model" object
        private class DataItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }    
        
        // We define our "data object" to bind our list
        // As you can see the ItemListAdapter is a generic type class        
        private class DataItemListAdapter : ItemListAdapter<Reply>
        {
            // We need expose a public constructor which basically set some base informations
            // like what Fragment will host this data source, the name of the .axml that will
            // be used to create all the View instances (with reusability) and the data source         
            public DataItemListAdapter(Activity context, IEnumerable<Reply> source)
                : base(context, Resource.Layout.CellDataItem, source)
            {
            } 
            
            // We need to override this method to get the instance of the View that will be rendered
            // and to bind data to its content            
            public override View GetView(int position, DataItem item, View view, ViewGroup parent)
            {
                // Do any binding o UI logic here
                
                return view;
            }
        }
        
        #endregion
        
        #region Fragment Methods
        
        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);
            
            var source = new DataItemListAdapter(this, new DataItems[]
            {
                new DataItem { Title = "Title1", Description = "Lore ipsum dolor sit" },
                new DataItem { Title = "Title2", Description = "Lore ipsum dolor sit" },
                new DataItem { Title = "Title3", Description = "Lore ipsum dolor sit" },
                new DataItem { Title = "Title4", Description = "Lore ipsum dolor sit" }
            });            
            
            this.DataItemList.Adapter = source;
            this.DataItemList.ItemSelected += DataItemList_ItemSelected;
        }
        
        public override void OnDestroyView()
        {
            base.OnDestroyView();
            
            this.DataItemList.ItemSelected -= DataItemList_ItemSelected;
        }
        
        #endregion
        
        #region Event Handlers
        
        private void SupportRequestList_ItemSelected(object sender, ItemListSelectEventArgs e)
        {
            DataItem item = e.Item as DataItem;
            // Do whatever you need with the selected DataItem        
        }
        
        #endregion
    }
```

#### Gestures
___
Gestures are a fashion new way to capture user input, in this era of  _touchable screen_ devices. Either **iOS** and **Android** support you with a very complete API, but as we already said before, we need a _common_ way to do this in a _easy-porting_ way.

##### The iOS Way
In **iOS** it's quite easy achieving this. Every **UIView** class based widget expose a bunch of methods to intercept user gestures via the _gesture recognizer_ model. Each _gesture_ is handled via a specific _gesture recognizer_ object that you need to assign to the **UIView** class base widget you want to monitor.

Let's see some code in action:
```cs
    public partial class SubViewController : AdMaiora.AppKit.UI.App.UISubViewController
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            
            // This will add a "tap" gesture recognizer for the ClickableView which is
            // a normal  UIView that can contain others widgets
            this.ClickableView.AddGestureRecognizer(new UITapGestureRecognizer(
                () =>
                {
                    // Do anything you need when the user "tap" the ClickableView!
                }));        
        }
    }
```

##### The Android Way
In **Android** it's not so straightforward to manage user gestures over a specific **View** class based widget. Or, at least, some gestures are easy to implement (like "tap" or "dobule tap"), but some others are very complicated. You have to deal with some maths and specific events.

**AppKit.UI** comes in your help here exposing the **GestureListener** class that is no more than a _factory object_ used to easly create _touch listener_ objects to assign to the **View** class base widget you want to monitor.

Let's see some code in action:
```cs
    public class SubFragment : AdMaiora.AppKit.UI.App.Fragment
    {
        public override void OnCreateView(LayoutInflater inflater, ViewGroup container)
        {
            base.OnCreateView(inflater, container);
            
            this.ClickableView.SetOnTouchListener(GestureListener.ForSingleTapUp(this.Activity,
                (e) =>
                {
                    // Do anything you need when the user "tap" the ClickableView!
                }));            
        }
    }
```

## In Conclusion
This is a basic documentation. We suggest to you to check our app examples which fully implement the **AppKit.UI** library.

All source code is available, fully forkable, modificable and playable in **GitHub**

Have a look to:
- [Chatty](https://github.com/admaiorastudio/chatty) a simple chat application
- [Listy](https://github.com/admaiorastudio/listy) a simple todo list application
- [Bugghy](https://github.com/admaiorastudio/bugghy) a simple bug tracking application