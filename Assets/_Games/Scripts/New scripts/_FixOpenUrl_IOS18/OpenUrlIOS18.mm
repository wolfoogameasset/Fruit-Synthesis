#import <UIKit/UIKit.h>

extern "C" {
    void OpenURLiOS(const char* url) {
        NSString* nsUrl = [NSString stringWithUTF8String:url];
        NSURL* parsedUrl = [NSURL URLWithString:nsUrl];

        if ([[UIApplication sharedApplication] canOpenURL:parsedUrl]) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [[UIApplication sharedApplication] openURL:parsedUrl options:@{} completionHandler:nil];
            });
        }
    }
}