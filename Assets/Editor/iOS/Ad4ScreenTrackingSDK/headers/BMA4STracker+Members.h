//
//  BMA4STracker+Members.h
//  Ad4ScreenTrackingSDK
//
//  Copyright (c) 2010-2014 Ad4Screen. All rights reserved.
//

#import "BMA4STracker.h"

typedef void(^BMA4SMemberListHandler)(NSArray* members, BOOL remoteMembers);

/**
 Use this class to track what you want with a specified memberID
 */
@interface BMA4STracker (Members)

/**
 Set a user active on the SDK. Only one user can be active at the same time.
 @param memberId The id of the user you use in you app.
 */
+(void)login:(NSString*)memberId;

/**
  Set the current active user to inactive on the SDK.
 */
+(void)logout;

/**
 Return the member who is logged in. If no user is already connected, return nil.
 */
+(NSString*)getActiveMember;

/**
 Return an array of the users who are registered in the SDK.
 */
+(void)listMembers:(BMA4SMemberListHandler)onSucceeded usingRemoteInformation:(BOOL)remote;

/**
 To remove (1,N) users of the SDK. If a user is already active, it will be automatically inactive.
 @param members An array of the idMember you want to delete in the sdk. Call this method when you remove members in your app.
 */
+(void)removeMembers:(NSArray*)members;

/**  Synchronize new value of the member profile
 @param values This Values is a NSDictionary with values that you want to synchronize
 
 NSDictionary* profile = [NSDictionary dictionaryWithObjectsAndKeys: @"John", @"user", nil];
 [BMA4STracker updateMemberInfo:profile];
 
 */
+ (void)updateMemberInfo:(NSDictionary*)values;

@end
