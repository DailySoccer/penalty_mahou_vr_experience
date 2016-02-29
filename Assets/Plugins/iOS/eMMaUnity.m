//
//  eMMaUnity.m
//  Unity-iPhone
//
#import <stdio.h>
#include "eMMa.h"

void eMMaUnity_startSession(unsigned char* apiKey)
{
  NSString *str = [NSString stringWithUTF8String:apiKey];
  [eMMa starteMMaSession:str];
}

unsigned char* eMMaUnity_getSDKVersion()
{
  NSString *str = [eMMa getSDKVersion];
  return NULL;
}
