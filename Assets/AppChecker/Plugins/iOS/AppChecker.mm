  //  Created by OJ on 7/13/16. http://answers.unity3d.com/questions/1217507/get-objective-c-functionvalue-in-c-wrapper-for-uni.html
 //  Modified by Poisins on 9 Mar 2017. Almost nothing left from original author :D
 //  In unity, You'd place this file in your "Assets>plugins>ios" folder

//Objective-C Code
#import <Foundation/Foundation.h>    

  @interface AppChecker:NSObject
  /* method declaration */
- (int)isAppInstalledX: (NSString*) urlSchemeName; 
  @end
  
  @implementation AppChecker

- (int)isAppInstalledX: (NSString*) urlSchemeName
   {
       int param = 0;
    
        if ([[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:[NSString stringWithFormat: @"%@:", urlSchemeName]]])
        {
            param = 1;
        }else{
            param = 0;
        }
        
        return param;
    }

  @end

 //C-wrapper that Unity communicates with
  extern "C"
  {

      int checkApp(const char *urlScheme)
      {
          AppChecker *status = [[AppChecker alloc] init];
          NSString *str = [NSString stringWithUTF8String:urlScheme];
          return [status isAppInstalledX:str];
      }
      
  }
