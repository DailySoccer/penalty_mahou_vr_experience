//
//  eMMa.h
//  eMMa
//
//  Created by GERARD LLORENTE VIVES on 02/11/11.
//  Copyright (c) 2011 Gerard Llorente. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface eMMa : NSObject

///---------------------------------------------------------------------------------------
/// @name Init eMMa
///---------------------------------------------------------------------------------------
+(void)starteMMaSession:(NSString*)appKey;


///---------------------------------------------------------------------------------------
/// @name eMMa configuration
///---------------------------------------------------------------------------------------
+(NSString*)getSDKVersion; //Get current SDK version

+(void)setDebuggerOutput:(BOOL)visible; //Show or hide debugger outputs

+(void)trackLocation; //Track location

+(void)seteMMaRootViewController:(UIViewController*)viewController; //Configures eMMa Root View Controller

+(void)setUseIDFA:(BOOL)use;

///---------------------------------------------------------------------------------------
/// @name eMMa User behaviour
///---------------------------------------------------------------------------------------

//Event tracking
+(void)trackEvent:(NSString*)event;


///---------------------------------------------------------------------------------------
/// @name eMMa User Stats
///---------------------------------------------------------------------------------------

//Login
+(void)loginUserID:(NSString*)userId forMail:(NSString*)mail andExtras:(NSDictionary*)extras;

+(void)loginUserID:(NSString*)userId forMail:(NSString*)mail;

+(void)loginDefault;

//Register. Lead generation
+(void)registerUserID:(NSString*)userId forMail:(NSString*)mail andExtras:(NSDictionary*)extras;

+(void)registerUserID:(NSString*)userId forMail:(NSString*)mail;

//Custom segmentation
+(void)trackExtraUserInfo:(NSDictionary*)info;

///---------------------------------------------------------------------------------------
/// @name eMMa m-Commerce
///---------------------------------------------------------------------------------------

//Start an order
+(void)startOrder:(NSString*)orderId customerId:(NSString*)customerId totalPrice:(float)totalPrice coupon:(NSString*)coupon extras:(NSDictionary*)extras;

+(void)startOrder:(NSString*)orderId customerId:(NSString*)customerId totalPrice:(float)totalPrice coupon:(NSString*)coupon;

+(void)startOrder:(NSString*)orderId customerId:(NSString*)customerId totalPrice:(float)totalPrice extras:(NSDictionary*)extras;

+(void)startOrder:(NSString*)orderId customerId:(NSString*)customerId totalPrice:(float)totalPrice;

+(void)startOrder:(NSString*)orderId totalPrice:(float)totalPrice;

+(void)startOrder:(NSString*)orderId totalPrice:(float)totalPrice coupon:(NSString*)coupon extras:(NSDictionary*)extras;

+(void)startOrder:(NSString*)orderId totalPrice:(float)totalPrice coupon:(NSString*)coupon;

+(void)startOrder:(NSString*)orderId totalPrice:(float)totalPrice extras:(NSDictionary*)extras;

//Add products to an existent order
+(void)addProduct:(NSString*)productId name:(NSString*)name qty:(float)qty price:(float)price extras:(NSDictionary*)extras;

+(void)addProduct:(NSString*)productId name:(NSString*)name qty:(float)qty price:(float)price;

//Track order
+(void)trackOrder;

//Set currency
+(void)setCurrency:(NSString*)currency;

//Cancel order by id
+(void)cancelOrder:(NSString*)orderId;

///---------------------------------------------------------------------------------------
/// @name eMMa Promo Web View
///---------------------------------------------------------------------------------------

+(void)checkForWebview;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button autoCreation:(BOOL)autoCreation;

+(void)checkForWebviewWithAutoCreation:(BOOL)autoCreation;

+(void)checkForWebviewWithParams:(NSDictionary*)params;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button params:(NSDictionary*)params;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button autoCreation:(BOOL)autoCreation params:(NSDictionary*)params;

+(void)checkForWebviewWithAutoCreation:(BOOL)autoCreation params:(NSDictionary*)params;

+(void)checkForWebviewWithLabel:(NSString*)label;

+(void)checkForWebviewWithParams:(NSDictionary*)params label:(NSString*)label;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button params:(NSDictionary*)params label:(NSString*)label;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button label:(NSString*)label;

+(void)checkForWebviewWithDelegate:(id)delegate;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button withDelegate:(id)delegate;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button autoCreation:(BOOL)autoCreation withDelegate:(id)delegate;

+(void)checkForWebviewWithAutoCreation:(BOOL)autoCreation withDelegate:(id)delegate;

+(void)checkForWebviewWithParams:(NSDictionary*)params withDelegate:(id)delegate;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button params:(NSDictionary*)params withDelegate:(id)delegate;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button autoCreation:(BOOL)autoCreation params:(NSDictionary*)params withDelegate:(id)delegate;

+(void)checkForWebviewWithAutoCreation:(BOOL)autoCreation params:(NSDictionary*)params withDelegate:(id)delegate;

+(void)checkForWebviewWithLabel:(NSString*)label withDelegate:(id)delegate;

+(void)checkForWebviewWithParams:(NSDictionary*)params label:(NSString*)label withDelegate:(id)delegate;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button params:(NSDictionary*)params label:(NSString*)label withDelegate:(id)delegate;

+(void)checkForWebviewWithCustomCloseButton:(UIButton*)button label:(NSString*)label withDelegate:(id)delegate;

+(void)closeWebview;

///---------------------------------------------------------------------------------------
/// @name eMMa AdBall
///---------------------------------------------------------------------------------------

+(void)checkForAdBall:(UIWindow*)window;

+(void)checkForAdBall:(UIWindow*)window autoCreation:(BOOL)autoCreation label:(NSString*)label;

+(void)checkForAdBall:(UIWindow*)window autoCreation:(BOOL)autoCreation;

+(void)checkForAdBall:(UIWindow*)window label:(NSString*)label;

///---------------------------------------------------------------------------------------
/// @name eMMa Banner
///---------------------------------------------------------------------------------------

+ (void)checkForBannerOnViewController:(UIViewController*)viewController autoCreation:(BOOL)autoCreation label:(NSString*)label;

+ (void)checkForBannerOnViewController:(UIViewController*)viewController;

+ (void)checkForBannerOnViewController:(UIViewController*)viewController label:(NSString*)label;

+ (void)checkForBannerOnViewController:(UIViewController*)viewController autoCreation:(BOOL)autoCreation;

///---------------------------------------------------------------------------------------
/// @name eMMa Strip
///---------------------------------------------------------------------------------------


+ (void)checkForStrip;

+ (void)checkForStripWithAutoCreation:(BOOL)autoCreation label:(NSString*)label;

+ (void)checkForStripWithAutoCreation:(BOOL)autoCreation;

+ (void)checkForStripWithLabel:(NSString*)label;



///---------------------------------------------------------------------------------------
/// @name eMMa Tab Bar View
///---------------------------------------------------------------------------------------

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index withTabBarItem:(UITabBarItem*)tabBarItem;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index withTabBarItem:(UITabBarItem*)tabBarItem autoCreation:(BOOL)autoCreation;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index autoCreation:(BOOL)autoCreation;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController autoCreation:(BOOL)autoCreation;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index withTabBarItem:(UITabBarItem*)tabBarItem params:(NSDictionary*)params;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index params:(NSDictionary*)params;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController params:(NSDictionary*)params;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index withTabBarItem:(UITabBarItem*)tabBarItem autoCreation:(BOOL)autoCreation params:(NSDictionary*)params;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController atIndex:(NSInteger)index autoCreation:(BOOL)autoCreation params:(NSDictionary*)params;

+(void)checkForPromoOnTabBar:(UITabBarController*)tabBarController autoCreation:(BOOL)autoCreation params:(NSDictionary*)params;


///---------------------------------------------------------------------------------------
/// @name eMMa Rate Alert
///---------------------------------------------------------------------------------------

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL timeFrequence:(int)timeFrequence withTitle:(NSString*)title message:(NSString*)message cancelButton:(NSString*)cancelButton rateItButton:(NSString*)rateItButton laterButton:(NSString*)laterButton;

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL withTitle:(NSString*)title message:(NSString*)message cancelButton:(NSString*)cancelButton rateItButton:(NSString*)rateItButton laterButton:(NSString*)laterButton;

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL timeFrequence:(int)timeFrequence;

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL;

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL timeFrequence:(int)timeFrequence withTitle:(NSString*)title message:(NSString*)message cancelButton:(NSString*)cancelButton rateItButton:(NSString*)rateItButton laterButton:(NSString*)laterButton appUpdate:(BOOL)appUpdate;

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL withTitle:(NSString*)title message:(NSString*)message cancelButton:(NSString*)cancelButton rateItButton:(NSString*)rateItButton laterButton:(NSString*)laterButton appUpdate:(BOOL)appUpdate;

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL timeFrequence:(int)timeFrequence appUpdate:(BOOL)appUpdate;

+(void)addRateAlertForAppStoreURL:(NSString*)appStoreURL appUpdate:(BOOL)appUpdate;


///---------------------------------------------------------------------------------------
/// @name eMMa Push system
///---------------------------------------------------------------------------------------

//Init push system
+(void)startPushSystem:(NSDictionary*)options;

+(void)startPushSystem:(NSDictionary*)options withDelegate:(id)delegate;

+(void)startPushSystem:(NSDictionary*)options showAlert:(BOOL)showAlert;

+(void)startPushSystem:(NSDictionary*)options withDelegate:(id)delegate showAlert:(BOOL)showAlert;

//Token registration
+(void)registerToken:(NSData*)deviceToken;

//Push registration
+(void)checkReceivedNotifications:(NSDictionary *)options;

///---------------------------------------------------------------------------------------
/// @name eMMa User Info
///---------------------------------------------------------------------------------------

+(NSString*)getUserID;

+(NSDictionary*)getUserInfo;

+(void)getUserInfoAsync:(void (^)(id))resultBlock;

+(void)getUserIDAsync:(void (^)(id))resultBlock;



@end
