//
//  BMA4SInAppNotification.h
//  Ad4ScreenTrackingSDK
//
//  Copyright (c) 2010-2014 Ad4Screen. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>
#import <UIKit/UIKit.h>

extern NSString *const BMA4SInAppNotification_Clicked;
extern NSString *const BMA4SInAppNotification_DidAppear;
extern NSString *const BMA4S_InAppNotification_Closed;
extern NSString *const BMA4S_InAppNotification_DataAvailable;
// Key used to store NSData received as an inAppNotification
extern NSString *const BMA4S_InAppNotification_DataKey;

/**
 You can use this class to manage inApp notification
 */
@interface BMA4SInAppNotification : NSObject

/**
 By default, the notifications are displayed at the bottom of the screen in Portrait mode.
 @param position CGPoint value for the position (x,y) of the banner in the screen.
 

 To change this position, you have to call setDefaultBannerOriginForPortrait with the required CGPoint.
 
    [BMA4SInAppNotification setDefaultBannerOriginForPortrait:CGPointMake(0, 200)];
 
 */
+ (void) setDefaultBannerOriginForPortrait:(CGPoint)position;

/**
 By default, the notifications are displayed at the bottom of the screen in Landscape mode.
 @param position CGPoint value for the position (x,y) of the banner in the screen.

 To change this position, you have to call setDefaultBannerOriginForPortrait with the required CGPoint.
 
    [BMA4SInAppNotification setDefaultBannerOriginForLandscape(0, 200)];
 
 */
+ (void) setDefaultBannerOriginForLandscape:(CGPoint)position;

/**
 Reset the default position of banner in the screen in portrait mode.
 */
+ (void) resetDefaultBannerOriginForPortrait;

/**
  Reset the default position of banner in the screen in landscape mode.
 */
+ (void) resetDefaultBannerOriginForLandscape;


/**
 You can prevent the display of any inApp notification, by calling this class method.
 This value can be changed at any time.
 @param locked Boolean value:
 
    - YES: if you want to disable inAppNotification
    - NO: to enable inAppNotification

 */
+ (void) setNotificationLock:(BOOL)locked;


/**
 Call this class method to get the status of notificationLock
 @return Return the state of notificationLock
 */
+ (BOOL) notificationLock;

/** 
 Set minimum time interval between display of 2 inApp notifications in a session
 @param minimumTimeInterval Minimum time interval between display of 2 inApp notifications
 */
+ (void) setNotificationTimeInterval:(NSTimeInterval)minimumTimeInterval;


@end