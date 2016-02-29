
//
//  Ad4ScreenController.m
//  Unity-iPhone
//
//  Created by Santiago Revelo Campos on 30/04/14.
//
//

#import "Ad4ScreenController.h"

#import "BMA4STracker.h"
#import "BMA4SInAppNotification.h"
/*#import "Ad4ScreenTrackingSDK/headers/BMA4SNotification.h"*/

@implementation Ad4ScreenController

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    NSLog( @"Ad4ScreenController init" );
    
    [BMA4STracker setDebugMode:YES];
    
    [BMA4SInAppNotification setNotificationLock:NO];
    
    // launch tracking
    [BMA4STracker trackWithPartnerId:@"mobileup343774dc8e998ed" privateKey:@"61bf02c070dfa8f7ab17156eb7a5c10b8a77bd51" options:launchOptions];
    
    // Add the view controller's view to the window and display.
   // [window addSubview:viewController.view];
   // [window makeKeyAndVisible];
    
    
    return [super application:application didFinishLaunchingWithOptions:launchOptions];
}
/*
-(BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
    return [[BMA4SNotification sharedBMA4S] applicationHandleOpenUrl:url];
}
*/
@end
