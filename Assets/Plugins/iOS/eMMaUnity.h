//
//  eMMaUnity.h
//  Unity-iPhone
//

#ifndef Unity_iPhone_eMMaUnity_h
#define Unity_iPhone_eMMaUnity_h



extern "C" {
  void eMMaFlurry_startSession(unsigned char* apiKey);
  unsigned char* eMMaFlurry_getSDKVersion(); // need BSTR may be?
}

#endif
