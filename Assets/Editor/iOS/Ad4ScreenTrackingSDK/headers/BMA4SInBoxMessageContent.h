//
//  BMA4SInBoxMessageContent.h
//  Ad4ScreenTrackingSDK
//
//  Copyright (c) 2010-2014 Ad4Screen. All rights reserved.
//

#import <Foundation/Foundation.h>
/**
 Define the type of message
 */
typedef NS_ENUM (NSUInteger, BMA4SMessageContentType){
    /**
     Define the type of message: Text
     */
    BMA4SMessageContentTypeText,
    /**
     Define the type of message: Web
     */
    BMA4SMessageContentTypeWeb
};

/**
 This object represents the content of an InBox message
 */
@interface BMA4SInBoxMessageContent : NSObject

/**
 This property define the type of message (web/text)
 */
@property (nonatomic, readonly) BMA4SMessageContentType type;

/**
 This is the body of the message text
 */
@property (nonatomic, readonly) NSString* body;

/**
 NSArray of buttons who appears on the message
 */
@property (nonatomic, readonly) NSArray* buttons;

@end
