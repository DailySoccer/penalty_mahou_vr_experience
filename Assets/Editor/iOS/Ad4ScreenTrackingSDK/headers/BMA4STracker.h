//
//  BMA4STracker.h
//  Ad4ScreenTrackingSDK
//
//  Copyright (c) 2010-2014 Ad4Screen. All rights reserved.
//

#import <Foundation/Foundation.h>

/** This class controller is the main controller of the SDK. You need to call the following method to launch the tracker
 
    + (void) trackWithPartnerId:(NSString*)partnerId privateKey:(NSString*)privateKey options:(NSDictionary *)launchOptions;

 */
@interface BMA4STracker : NSObject

/** Call this class method to track your application.
@param partnerId That we have provided
@param privateKey That we have provided
@param launchOptions The same of the didFinishLaunchingWithOptions method.
 Call this class method once in this following method of your app delegate (recommended) to track your application.
 
 - (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
 
*/
+ (void) trackWithPartnerId:(NSString*)partnerId privateKey:(NSString*)privateKey options:(NSDictionary *)launchOptions;

/** Call this class method to report analytics data.
 
 You can send events that will be reported to our servers. Event logs and statistics will be available on our back office.
 @param eventType The event type is an integer defining the type of event. The values below 1000 are reserved for Ad4Screen usage. You can use custom event type starting from 1001. The event types will be helpful to track specific events of the developer's choice and report them on Ad4Screen back office.
 @param parameters This Value is an array of string. The content of the array is free, as long as you use strings inside. All the strings in the array will be sent. For example, you can use keys/values, separated by a semicolon.
 
    // example of event. We want to send a custom event at start
    NSArray *array = [NSArray arrayWithObject:@"ApplicationStart=successful;userId=000"];
    [BMA4STracker trackEventWithType:1001 parameters:array];
 
 */
+ (void) trackEventWithType:(NSInteger) eventType parameters:(NSArray*) parameters;

/** Call this class method to report your own user ID to our server.
 @param userID Whatever you want, that can be the ID of the user in your database. It's recommended to have a different userID for each user of your app.
 
Max length: 128 bytes
 */
+ (void) setUserID:(NSString*)userID;

/** Call this class method to return the reported own userID
  @return The reported own userID as a NSString
 */
+ (NSString*) userID;

/** Call this class method to return Ad4Screen internal ID 
 @return The Ad4Screen internal ID as a NSString (Mobinaute ID)
 */
+ (NSString*) A4SID;

/**  Synchronize new value of the device profile
 @param values This Values is a NSDictionary with values that you want to synchronize
 
 NSDictionary* profile = [NSDictionary dictionaryWithObjectsAndKeys: [NSNumber numberWithInt:1000], @"id",
 @"iPhone5s", @"deviceModel", nil];
 [BMA4STracker updateDeviceInfo:profile];
 
 */
+ (void) updateDeviceInfo:(NSDictionary*)values;

/** Active the log in console to see what appends with the Ad4Screen SDK.
 @param value YES or NO
 */
+ (void) setDebugMode:(BOOL)value;

/** The setDoNoTrack method is persistent. If you deactivate the SDK, at next launching it will still be deactivated. Even if you call [BMA4StrackWithPartnerId…]. 
 You’ll have to call setDoNotTrack:NO before.
 
 Setting doNotTrack to YES will stop the SDK. But setting to NO will no relaunch the SDK. It is recommended to wait next launch to restart the sdk
 @param value YES or NO
 */
+ (void) setDoNotTrack:(BOOL)value;


/**  Use this method to know if the tracking is enabled
 @return A state of the tracking. YES if SDK is disabled.
 Relaunch the SDK if you want to reactivate it
 */
+ (BOOL) doNotTrack;

@end