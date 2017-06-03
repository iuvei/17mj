//
//  testplugin1.m
//  testplugin1
//
//  Created by 朱賢譯 on 2017/5/18.
//  Copyright © 2017年 朱賢譯. All rights reserved.
//

#import "testplugin1.h"

extern UIViewController* UnityGetGLViewController();
extern void UnitySendMessage(const char *, const char *, const char *);

@implementation testplugin1

+ (testplugin1 *)GetSharedInstance {
    static testplugin1 *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[testplugin1 alloc] init];
        
    });
    return sharedInstance;
}

-(void)initRecord{
    NSError *goCoderLicensingError = [WowzaGoCoder registerLicenseKey:@"GOSK-B543-0103-39FC-DA5D-0BD0"];
    if (goCoderLicensingError != nil) {
        // Log license key registration failure
        NSLog(@"%@", [goCoderLicensingError localizedDescription]);
    } else {
        // Initialize the GoCoder SDK
        self.goCoder = [WowzaGoCoder sharedInstance];
    }
    
    if (self.goCoder != nil) {
        // Associate the U/I view with the SDK camera preview
        CGRect viewRect = CGRectMake(200, 0, 300, 190);
        self.unityView = UnityGetGLView();
        self.videoView = [[UIView alloc] initWithFrame:viewRect];
        self.goCoder.cameraView = self.videoView;
        
        // Set the active camera to the front camera if it is not already active
        if (self.goCoder.cameraPreview.camera.direction != WZCameraDirectionFront)
            [self.goCoder.cameraPreview switchCamera];
        
        // Start the camera preview
        [self.goCoder.cameraPreview startPreview];
    }
    
    // Get a copy of the active config
    WowzaConfig *goCoderBroadcastConfig = self.goCoder.config;
    
    // Set the defaults for 720p video
    [goCoderBroadcastConfig loadPreset:WZFrameSizePreset352x288];
    
    // Set the connection properties for the target Wowza Streaming Engine server or Wowza Cloud account
    goCoderBroadcastConfig.hostAddress = @"192.168.20.171";
    goCoderBroadcastConfig.portNumber = 1935;
    goCoderBroadcastConfig.applicationName = @"17MJ";
    goCoderBroadcastConfig.streamName = @"myStream";
    //goCoderBroadcastConfig.username = @"orson";
    //goCoderBroadcastConfig.password = @"orson168";
    // Update the active config
    self.goCoder.config = goCoderBroadcastConfig;
    //[self.broadcastButton addTarget:self action:@selector(broadcastButtonTapped:)
    //               forControlEvents:UIControlEventTouchUpInside];
    
}

-(void)tapRecord{
    // Ensure the minimum set of configuration settings have been specified necessary to
    // initiate a broadcast streaming session
    NSError *configValidationError = [self.goCoder.config validateForBroadcast];
    
    if (configValidationError != nil) {
        UIAlertView *alertDialog =
        [[UIAlertView alloc] initWithTitle:@"Incomplete Streaming Settings"
                                   message: self.goCoder.status.description
                                  delegate:nil
                         cancelButtonTitle:@"OK"
                         otherButtonTitles:nil];
        [alertDialog show];
    } else if (self.goCoder.status.state != WZStateRunning) {
        // Start streaming
        [self.goCoder startStreaming:self];
        //[self.unityView addSubview:self.videoView];
        //self.unityView.layer.zPosition = 1;<#(nonnull UIView *)#>
        [self.unityView.superview insertSubview:self.videoView atIndex:1];
        NSLog(@"self.unityView.superviews %@", self.unityView.superview.subviews);
    }
    else {
        // Stop the broadcast that is currently running
        [self.goCoder endStreaming:self];
        [self.videoView removeFromSuperview];
        //[parent remove
    }
}
-(void)startRecord{
    // Ensure the minimum set of configuration settings have been specified necessary to
    // initiate a broadcast streaming session
    NSError *configValidationError = [self.goCoder.config validateForBroadcast];
    
    if (configValidationError != nil) {
        UIAlertView *alertDialog =
        [[UIAlertView alloc] initWithTitle:@"Incomplete Streaming Settings"
                                   message: self.goCoder.status.description
                                  delegate:nil
                         cancelButtonTitle:@"OK"
                         otherButtonTitles:nil];
        [alertDialog show];
    } else if (self.goCoder.status.state != WZStateRunning) {
        // Start streaming
        [self.goCoder startStreaming:self];
        //[self.unityView addSubview:self.videoView];
        [self.unityView.superview addSubview:self.videoView];
        //NSString *str =[[NSString alloc] init];
        //NSString *url = [[NSString alloc] init];
        NSString *url = @"rtsp://192.168.20.238:1935/17MJ";
        //self.goCoder.config.hostAddress
        UnitySendMessage("GameManager", "OnRecordStart", url.UTF8String);
        //self.unityView.layer.zPosition = 1;<#(nonnull UIView *)#>
        //[self.unityView.superview insertSubview:self.videoView atIndex:1];
        //NSLog(@"self.unityView.superviews %@", self.unityView.superview.subviews);
    }
}
-(void)stopRecord{
    // Ensure the minimum set of configuration settings have been specified necessary to
    // initiate a broadcast streaming session
    NSError *configValidationError = [self.goCoder.config validateForBroadcast];
    
    if (configValidationError != nil) {
        UIAlertView *alertDialog =
        [[UIAlertView alloc] initWithTitle:@"Incomplete Streaming Settings"
                                   message: self.goCoder.status.description
                                  delegate:nil
                         cancelButtonTitle:@"OK"
                         otherButtonTitles:nil];
        [alertDialog show];
    } else if (self.goCoder.status.state == WZStateRunning) {
        // Stop the broadcast that is currently running
        [self.goCoder endStreaming:self];
        [self.videoView removeFromSuperview];
        //[parent remove
    }
}
- (void) onWZStatus:(WZStatus *) goCoderStatus {
    // A successful status transition has been reported by the GoCoder SDK
    NSString *statusMessage = nil;
    
    switch (goCoderStatus.state) {
        case WZStateIdle:
            statusMessage = @"The broadcast is stopped";
            break;
            
        case WZStateStarting:
            statusMessage = @"Broadcast initialization";
            break;
            
        case WZStateRunning:
            statusMessage = @"Streaming is active";
            break;
            
        case WZStateStopping:
            statusMessage = @"Broadcast shutting down";
            break;
    }
    
    if (statusMessage != nil)
        NSLog(@"Broadcast status: %@", statusMessage);
}

- (void) onWZError:(WZStatus *) goCoderStatus {
    // If an error is reported by the GoCoder SDK, display an alert dialog
    // containing the error details using the U/I thread
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertDialog =
        [[UIAlertView alloc] initWithTitle:@"Streaming Error"
                                   message:goCoderStatus.description
                                  delegate:nil
                         cancelButtonTitle:@"OK"
                         otherButtonTitles:nil];
        [alertDialog show];
    });
}

@end



// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
#if __cplusplus
extern "C" {
#endif
    //-------------------------------------------------------------------------------------------------
    void _initRecord(){
        testplugin1 *objVideoCapture = [testplugin1 GetSharedInstance];
        [objVideoCapture initRecord];
    }
    //-------------------------------------------------------------------------------------------------
    void _tapRecord (){
        testplugin1 *objVideoCapture = [testplugin1 GetSharedInstance];
        [objVideoCapture tapRecord];
    }
    void _stopRecord (){
        testplugin1 *objVideoCapture = [testplugin1 GetSharedInstance];
        [objVideoCapture stopRecord];
    }
    void _startRecord (){
        testplugin1 *objVideoCapture = [testplugin1 GetSharedInstance];
        [objVideoCapture startRecord];
    }
#if __cplusplus
}
#endif
