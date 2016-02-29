//
//  BMA4SInBoxMessage.h
//  Ad4ScreenTrackingSDK
//
//  Copyright (c) 2010-2014 Ad4Screen. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "BMA4SInBoxMessageContent.h"

extern NSString *const BMA4SInBoxMessageWillDisplay;
extern NSString *const BMA4SInBoxMessageWillInteract;

@class BMA4SInBoxMessageContent;
@class BMA4SInBoxMessage;

typedef void (^BMA4SInBoxDisplayHandler)(BMA4SInBoxMessage* message, BMA4SInBoxMessageContent* content);
/**
 Define a BMA4SInBoxMessage object with her properties and method instance
 */
@interface BMA4SInBoxMessage : NSObject

/**
 This property define the title of BMA4SInBoxMessage object
 */
@property (nonatomic, readonly) NSString* title;

/**
 This property define the text of BMA4SInBoxMessage object
 */
@property (nonatomic, readonly) NSString* text;

/**
 This property define the sender of BMA4SInBoxMessage object
 */
@property (nonatomic, readonly) NSString* from;

/**
 This property define the category of BMA4SInBoxMessage object
 */
@property (nonatomic, readonly) NSString* category;

/**
 This property define the date when the BMA4SInBoxMessage object was sent
 */
@property (nonatomic, readonly) NSDate* date;

/**
 This property define the url of the icon who appear in BMA4SInBoxMessage object
 */
@property (nonatomic, readonly) NSString* iconUrl;

/**
 This property define if the BMA4SInBoxMessage object is read by the user
 */
@property (nonatomic, readonly) BOOL isRead;

/**
 This property define if the BMA4SInBoxMessage object is expired
 */
@property (nonatomic, readonly) BOOL isExpired;

/**
 Call message 'interactWithDisplayHandler' method to hand the message to the SDK
 */
-(void)interactWithDisplayHandler:(BMA4SInBoxDisplayHandler)handler;


/**
 Call this method if you want to mark a BMA4SInBoxMessage object as read
 */
-(void)markAsRead;

/**
 Call this method if you want to mark a BMA4SInBoxMessage object as unread
 */
-(void)markAsUnread;

/**
 Call this method if you want to know if a BMA4SInBoxMessage object was archived
 */
-(BOOL)isArchived;

/**
  Call this method if you want to archive a BMA4SInBoxMessage object. A message archived will be deleted
 */
-(void)archive;

/**
  Call this method if you want to unarchive a BMA4SInBoxMessage object.
 
 A BMA4SInBoxMessage object can't be unarchive if was deleted
 */
-(void)unarchive;

@end
