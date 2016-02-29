//
//  BMA4SNotification.h
//  Ad4ScreenTrackingSDK
//
//  Copyright (c) 2010-2014 Ad4Screen. All rights reserved.
//

#import <Foundation/Foundation.h>

@class CLLocation;
@class UILocalNotification;


/** In your application delegate, add the following code :
 
    - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions {
 
        //Override point for customization after application launch.
 
        // Add the view controller's view to the window and display.
        [self.window addSubview:viewController.view];
        [self.window makeKeyAndVisible];
 
        [BMA4STracker trackWithPartnerId:@"your partner id" privateKey:@"your private key"];
 
        // register for notification
        [[UIApplication sharedApplication] registerForRemoteNotificationTypes:(UIRemoteNotificationTypeBadge |
        UIRemoteNotificationTypeSound | UIRemoteNotificationTypeAlert)];
 
        // if iOS launch the application because of a notification, handle it
        [[BMA4SNotification sharedBMA4S] didFinishLaunchingWithOptions:launchOptions controller:viewController];
 
        return YES;
    }
 
 
    - (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
    {
        [[BMA4SNotification sharedBMA4S] registerDeviceToken:deviceToken];
    }
 
    // called by iOS when we receive a notification
    - (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
    {
        [[BMA4SNotification sharedBMA4S] didReceiveRemoteNotification:userInfo];
    }
 */
@interface BMA4SNotification : NSObject 


/**
 Get the Singleton
 */
+ (BMA4SNotification*) sharedBMA4S;


/** 
 Register the user device token to Ad4Screen
 @param deviceToken This value is given by this following method in your appDelegate:
 
    - (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
 
 */
- (void) registerDeviceToken:(NSData*) deviceToken;

/**
 Check if the app was launch because of the reception of a notification, and display the notification if necessary
 @param launchOptions This value is given by this following method in your appDelegate:
 
    - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions

 */
- (void) didFinishLaunchingWithOptions:(NSDictionary *)launchOptions;


/**
 Display the notification
 @param userInfo This value is given by this following method in your appDelegate:
 
    - (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo
 
 */
- (void) didReceiveRemoteNotification:(NSDictionary *)userInfo;

/**
 Synchronize new value for the user profile
 @param values This Values is a NSDictionary with values that you want to synchronize
 
 NSDictionary* profile = [NSDictionary dictionaryWithObjectsAndKeys: [NSNumber numberWithInt:1000], @"id",
 @"john", @"user", nil];
 [[BMA4SNotification sharedBMA4S] synchroniseProfile:profile];
 
 @deprecated Since 4.0.0, use [BMA4STracker updateDeviceInfo:] instead.
 
 */
- (void) synchroniseProfile:(NSDictionary*)values __attribute__((deprecated("use [BMA4STracker updateDeviceInfo:] instead")));

/**
 You can prevent the display of any Push notification while displaying ads, by calling this instance method.
 This value can be changed at any time.
 @param value Boolean value:
 
 - YES: if you want to disable inAppNotification
 - NO: to enable inAppNotification
 
 */
- (void) setPushNotificationLock:(BOOL)value;

/**
 Call this instance method to get the status of pushNotificationLock
 @return Return a boolean value which represents the state of pushNotificationLock
 */
- (BOOL) pushNotificationLock;

/**
 If your application use geo-location (GPS), you can use the user's location to target your notification based on location criteria. 
 
 To update the location on our server you can call this instance method.
 @param location A CLLocation object which represent the location of the user
 */
- (void) updateLocation:(CLLocation*)location;

/**
 If TRUE, allow the SDK to change every inApp banner or rich push superView
 to ensure they are always above every other view. Default value is FALSE
 @param value A boolean value
 */
- (void) allowViewAutomaticalyOnTop:(BOOL)value;

/**
 This will allow the SDK to track and display local notification sent by Ad4Screen.
 
 Add this method code to your application delegate like this:
 
    - (void)application:(UIApplication *)application didReceiveLocalNotification:(UILocalNotification *)notification
    {
        [[BMA4SNotification sharedBMA4S] didReceiveLocalNotification:notification];
    }
 
 
 @param notification The same as didReceiveLocalNotification in your appDelegate
 */
- (void) didReceiveLocalNotification:(UILocalNotification *)notification;

/**
 Call this method in your appDelegate.
 @param url The same as applicationHandleOpenUrl where this method is called.
 */
- (BOOL) applicationHandleOpenUrl:(NSURL*)url;

/**
 Call this instance method to reset the Badge number of the user.
 */
- (void) resetBadgeNumber;

/**
 Call this instance method with value NO to prevent SDK from resetting application badge number automatically.
 */
- (void) setAllowBadgeReset:(BOOL)allow;

@end