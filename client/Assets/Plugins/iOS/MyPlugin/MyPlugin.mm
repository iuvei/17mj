//
//  MyPlugin.m
//  MyPlugin
//
//  Created by 朱賢譯 on 2017/6/5.
//  Copyright © 2017年 朱賢譯. All rights reserved.
//

#import "MyPlugin.h"

@implementation MyPlugin

static MyPlugin *_sharedInstance = nil;
static dispatch_once_t onceToken = 0;

+ (MyPlugin *)sharedInstance {
    dispatch_once(&onceToken, ^{
        _sharedInstance = [[MyPlugin alloc] init];
        
    });
    return _sharedInstance;
}

- (void)willStartWithViewController:(UIViewController*)controller {
    NSLog(@"willStartWithViewController()");
    //NSLog(@"%", controller);
    // 新建自定义视图控制器。
    //self.viewController = [[myViewController alloc] init];
    myViewController *viewController = [[myViewController alloc] init];
    //屏幕尺寸
    //CGFloat ApplicationW = [[UIScreen mainScreen] bounds].size.width;
    //CGFloat ApplicationH = [[UIScreen mainScreen] bounds].size.height;
    
    //viewController.view = [[UIView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    //[viewController.view setBackgroundColor:[UIColor greenColor]];
    //if(_unityView) {
    //[viewController.view setFrame:CGRectMake(0, 0, 500, 500)];
        // 把Unity的内容视图作为子视图放到我们自定义的视图里面。
        [viewController.view addSubview:_unityView];
    
        // 把Unity的内容视图作为子视图放到我们自定义的视图里面。
        //[self.viewController.view addSubview:_unityView];
        //[_unityView setFrame:CGRectMake(0, 0, ApplicationW/3*2, ApplicationH)];
        //_unityView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
    //}
    // 把根视图和控制器全部换成我们自定义的内容。
    _rootController = viewController;
    _rootView = _rootController.view;
    dispatch_once(&onceToken, ^{
        _sharedInstance = self;
        
    });
    //[_sharedInstance initRecord];
    //[_sharedInstance startRecord];
}

/*!
 * 推流错误
 */
- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session error:(NSError *)error{
    dispatch_async(dispatch_get_main_queue(), ^{
        NSString *msg = [NSString stringWithFormat:@"%zd %@",error.code, error.localizedDescription];
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Live Error" message:msg delegate:nil cancelButtonTitle:@"取消" otherButtonTitles:@"重新连接", nil];
        alertView.delegate = self;
        [alertView show];
    });
    
    NSLog(@"!!!error : %@", error);
}

- (void)alivcLiveVideoReconnectTimeout:(AlivcLiveSession*)session {
    
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"提示" message:@"重连超时（此处根据实际情况决定，默认重连时长5s，可更改，建议开发者在此处重连）" delegate:nil cancelButtonTitle:@"OK" otherButtonTitles: nil];
        
        [alertView show];
    });
    
}

- (void)alivcLiveVideoLiveSessionConnectSuccess:(AlivcLiveSession *)session {
    NSLog(@"connect success!");
}

- (void)alivcLiveVideoLiveSessionNetworkSlow:(AlivcLiveSession *)session{
    // 注意：一定要套 主线程 完成UI操作
    //dispatch_async(dispatch_get_main_queue(), ^{
        //self.textView.text = @"网速太慢";
    //});
    NSLog(@"网络很慢，已经不建议直播");
}

- (void)alivcLiveVideoOpenAudioSuccess:(AlivcLiveSession *)session {
    //dispatch_async(dispatch_get_main_queue(), ^{
    //    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"YES" message:@"麦克风打开成功" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
    //    [alertView show];
    //});
    NSLog(@"麦克风打开成功");
}

- (void)alivcLiveVideoOpenVideoSuccess:(AlivcLiveSession *)session {
    //dispatch_async(dispatch_get_main_queue(), ^{
    //    UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"YES" message:@"摄像头打开成功" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
    //    [alertView show];
    //});
    NSLog(@"摄像头打开成功");
}


- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session openAudioError:(NSError *)error {
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"麦克风获取失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
    NSLog(@"麦克风获取失败");
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session openVideoError:(NSError *)error {
    
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"摄像头获取失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
    NSLog(@"摄像头获取失败");
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session encodeAudioError:(NSError *)error {
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"音频编码初始化失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
    NSLog(@"音频编码初始化失败");
    
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session encodeVideoError:(NSError *)error {
    dispatch_async(dispatch_get_main_queue(), ^{
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:@"Error" message:@"视频编码初始化失败" delegate:nil cancelButtonTitle:@"确定" otherButtonTitles: nil];
        [alertView show];
    });
    NSLog(@"视频编码初始化失败");
}

- (void)alivcLiveVideoLiveSession:(AlivcLiveSession *)session bitrateStatusChange:(ALIVC_LIVE_BITRATE_STATUS)bitrateStatus {
    
    dispatch_async(dispatch_get_main_queue(), ^{
        NSLog(@"升降码率:%ld", bitrateStatus);
    });
}

- (void)timeUpdate{
    AlivcLDebugInfo *i = [self.liveSession dumpDebugInfo];
    NSDate *date = [NSDate dateWithTimeIntervalSince1970:i.connectStatusChangeTime];
    
    NSMutableString *msg = [[NSMutableString alloc] init];
    [msg appendFormat:@"CycleDelay(%0.2fms)\n",i.cycleDelay];
    [msg appendFormat:@"bitrate(%zd) buffercount(%zd)\n",[self.liveSession alivcLiveVideoBitRate] ,self.liveSession.dumpDebugInfo.localBufferVideoCount];
    [msg appendFormat:@" efc(%zd) pfc(%zd)\n",i.encodeFrameCount, i.pushFrameCount];
    [msg appendFormat:@"%0.2ffps %0.2fKB/s %0.2fKB/s\n", i.fps,i.encodeSpeed, i.speed/1024];
    [msg appendFormat:@"%lluB pushSize(%lluB) status(%zd) %@",i.localBufferSize, i.pushSize, i.connectStatus, date];
    [msg appendFormat:@" %0.2fms\n",i.localDelay];
    [msg appendFormat:@"video_pts:%zd\naudio_pts:%zd\n", i.currentVideoPTS,i.currentAudioPTS];
    [msg appendFormat:@"fps:%f\n", i.fps];
    NSLog(msg);
    //_textView.text = msg;
    
}

//-(void)alivcLiveVideoOpenVideoSuccess:(AlivcLiveSession *)session
//{
//    NSLog(@"推流连接成功");
//}

/*
-(void) showDemoView
{
    // if you displayed as a contained controller and removed it then self.presentedController will be nil
    if (self.presentedController == nil)
    {
        // either load as contained controller or as presented controller
        // change this to see both ways of displaying your content then remove
        // when you know what you want to use
        BOOL loadAsContained = true;
        
        // this view controller is defined in a storyboard, get a reference to the containing storyboard
        //UIStoryboard *storyboard = [UIStoryboard storyboardWithName:@"Main" bundle:[NSBundle mainBundle]];
        
        // instantiate the view controller from the storyboard
        //UIViewController *demo = [storyboard instantiateViewControllerWithIdentifier:@"DemoVC"];
        //UIViewController *demo = UIViewController.
        UIViewController *demo = [[UIViewController alloc] init];
        
        if (loadAsContained)
        {
            // add this view controller as a contained controller (child) of the presented view controller
            [self addContainedController:demo];
        }
        else
        {
            // if you don't want to display as a child, and instead want to present the view controller
            // on top of the currently presented controller then use this method instead of the previous one
            [[self getTopViewController] presentViewController:demo animated:YES completion:nil];
        }
    }
    else
    {
        [self removeContainedController:self.presentedController];
    }
}
*/
/*
- (UIViewController*) getTopViewController
{
    // get the top most window
    UIWindow *window = [self getTopApplicationWindow];
    
    // get the root view controller for the top most window
    UIViewController *vc = window.rootViewController;
    
    // check if this view controller has any presented view controllers, if so grab the top most one.
    while (vc.presentedViewController != nil)
    {
        // drill to topmost view controller
        vc = vc.presentedViewController;
    }
    
    return vc;
}

// condensed version
- (UIWindow*) getTopApplicationWindow
{
    // grabs the top most window
    NSArray* windows = [[UIApplication sharedApplication] windows];
    return ([windows count] > 0) ? windows[0] : nil;
}

-(void) addContainedController:(UIViewController*)controller
{
    // get a reference to the current presented view controller
    UIViewController *parent = [self getTopViewController];
    
    // notify of containment
    [controller willMoveToParentViewController:parent];
    
    // add content as child
    [parent addChildViewController:controller];
    
    // set frame of child content (for demo inset by 100px padding on all sides)
    controller.view.frame = CGRectMake(100.0, 100.0, parent.view.bounds.size.width - 200.0, parent.view.bounds.size.height - 200.0);
    
    // get fancy, lets animate in
    controller.view.alpha = 0.0;
    
    // add as subview
    [parent.view addSubview:controller.view];
    
    // animation duration
    CGFloat duration = 0.3;
    
    // animate the alpha in and bring top views to top
    [UIView animateWithDuration:duration
                     animations:^
     {
         controller.view.alpha = 1.0;
     }
                     completion:nil
     ];
    
    // set our tracker variable
    //self.presentedController = controller;
}

-(void) removeContainedController:(UIViewController*)controller
{
    // if fade out our view here just because
    [UIView animateWithDuration:0.3
                     animations:^
     {
         controller.view.alpha = 0;
     }
                     completion:^(BOOL finished)
     {
         
         // inform the child it is being removed by passing nil here
         [controller willMoveToParentViewController:nil];
         
         // remove the view
         [controller.view removeFromSuperview];
         
         // remove view controller from container
         [controller removeFromParentViewController];
         
         // nil out tracker
         self.presentedController = nil;
     }];
}
 */
/*
- (void)viewDidLoad {
    [super viewDidLoad];
    NSLog(@"viewDidLoad()");
    // Do any additional setup after loading the view.
    //[self.view addSubview:UnityGetGLView()];
    self.view.backgroundColor = [UIColor blueColor];
    //UnityGetGLView().frame = self.view.frame;
    
    UILabel *text = [[UILabel alloc] initWithFrame:CGRectMake(0, 0, 100, 100)];
    text.text = @"欢迎进入Unity界面！(IOS)";
    text.textAlignment = NSTextAlignmentCenter;
    text.backgroundColor = [UIColor greenColor];
    [self.view addSubview:text];
    
    //UITextField *tv = [[UITextField alloc] initWithFrame:CGRectMake(40, 100, 300, 40)];
    //tv.textAlignment = NSTextAlignmentCenter;
    //tv.layer.borderWidth = 1;
    //tv.layer.cornerRadius = 3;
    //[tv setPlaceholder:@"请键入文字..."];
    //tv.tag = 101;
    //[self.view addSubview:tv];
    
    //UIButton *btnNext = [UIButton buttonWithType:UIButtonTypeSystem];
    //[btnNext setTitle:@"点我可以修改CUBE中的文字(IOS)" forState:UIControlStateNormal];
    //btnNext.frame = CGRectMake(40, 150, 300, 44);
    //btnNext.backgroundColor = [UIColor whiteColor];
    //btnNext.layer.borderWidth = 1;
    //btnNext.layer.cornerRadius = 3;
    //[self.view addSubview:btnNext];
    
    //[btnNext addTarget:self action:@selector(goToLastScene:) forControlEvents:UIControlEventTouchUpInside];
    
    //UIButton *recevied = [UIButton buttonWithType:UIButtonTypeSystem];
    //[recevied setTitle:@"点我可以进入到另外一个IOS界面哦！(IOS)" forState:UIControlStateNormal];
    //recevied.frame = CGRectMake(40, 450, 300, 44);
    //recevied.backgroundColor = [UIColor whiteColor];
    //recevied.layer.borderWidth = 1;
    //recevied.layer.cornerRadius = 3;
    //    btnNext.center = CGPointMake(self.view.bounds.size.height / 2, self.view.bounds.size.width / 2);
    //[self.view addSubview:recevied];
    
    //[recevied addTarget:self action:@selector(recevied:) forControlEvents:UIControlEventTouchUpInside];
    
    
}

*/

// 设置直播参数
- (void)setupConfiguration {
    //1.初始化config配置类
    self.configuration = [[AlivcLConfiguration alloc]init];
    //2. 设置推流地址
    self.configuration.url = self.pushUrl;
    //3. 设置最大码率
    /*!
     *  最大码率，网速变化的时候会根据这个值来提供建议码率
     *  默认 1500 * 1000
     */
    self.configuration.videoMaxBitRate = 1500 * 1000;
    //4. 设置当前视频码率
    /*!
     *  默认码率，在最大码率和最小码率之间
     *  默认 600 * 1000
     */
    self.configuration.videoBitRate = 600 * 1000;
    //5. 设置最小码率
    /*!
     *  默认码率，在最大码率和最小码率之间
     *  默认 600 * 1000
     */
    self.configuration.videoMinBitRate = 400 * 1000;
    //6. 设置音频码率
    /*!
     *  音频码率
     *  默认 64 * 1000
     */
    self.configuration.audioBitRate = 64 * 1000;
    //7. 设置直播分辨率
    //self.configuration.videoSize = CGSizeMake(360, 640);
    self.configuration.videoSize = CGSizeMake(640, 360);
    //8. 设置横屏or竖屏 默认竖屏
    self.configuration.screenOrientation = AlivcLiveScreenHorizontal;
    //self.configuration.screenOrientation = AlivcLiveScreenVertical;
    //9. 设置帧率 default 20
    self.configuration.fps = 20;
    //10. 设置摄像头采集质量
    self.configuration.preset = AVCaptureSessionPresetiFrame1280x720;
    //11. 设置前置摄像头或后置摄像头
    self.configuration.position = AVCaptureDevicePositionFront;
    //12.设置水印图片 默认无水印
    //self.configuration.waterMaskImage = [UIImage imageNamed:@"watermask"];
    //13.设置水印位置
    //self.configuration.waterMaskLocation = 1;
    //14.设置水印相对x边框距离
    //self.configuration.waterMaskMarginX = 10;
    //15.设置水印相对y边框距离
    //self.configuration.waterMaskMarginY = 10;
    //16.设置重连超时时长
    self.configuration.reconnectTimeout = 5;
}

- (void)setupLiveSession {
    //1. 初始化liveSession类
    self.liveSession = [[AlivcLiveSession alloc]initWithConfiguration:self.configuration];
    //2. 设置session代理
    self.liveSession.delegate = self;
    //3. 开启直播预览
    [self.liveSession alivcLiveVideoStartPreview];
    //4. 推流连接
    [self.liveSession alivcLiveVideoConnectServer];
    CGFloat ApplicationW = [[UIScreen mainScreen] bounds].size.width;
    CGFloat ApplicationH = [[UIScreen mainScreen] bounds].size.height;
    NSLog(@"ApplicationW=%f, ApplicationH=%f", ApplicationW, ApplicationH);
    
    [self.liveSession.previewView setBounds: CGRectMake(0, 0, ApplicationH, ApplicationW/3)];
    //self.liveSession.previewView.autoresizesSubviews = YES;
    self.liveSession.previewView.transform = CGAffineTransformMakeRotation(M_PI/2);
    self.liveSession.previewView.layer.anchorPoint = CGPointMake(0.5, 1.5);
    
    //self.liveSession.previewView.layer.cornerRadius = self.liveSession.previewView.bounds.size.height /2;
    //self.liveSession.previewView.layer.masksToBounds = YES;
    //self.liveSession.previewView.layer.borderWidth = 1;
    //self.liveSession.previewView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
    //[self.view addSubview:[self.liveSession previewView]];
    
    //开启美颜
    [self.liveSession setEnableSkin:YES];
    //缩放
    [self.liveSession alivcLiveVideoZoomCamera:1.0f];
    //聚焦
    //[self.liveSession alivcLiveVideoFocusAtAdjustedPoint:percentPoint autoFocus:YES];
    //调试信息
    //AlivcLDebugInfo  *i = [self.liveSession dumpDebugInfo];
    //静音
    //[self.liveSession setMute:YES];
    
    //5. 非常重要
    dispatch_async(dispatch_get_main_queue(), ^{
        //    //[self.view insertSubview:[self.liveSession previewView] atIndex:0];
        //    [self.view addSubview:[self.liveSession previewView]];
        [_rootView addSubview: [self.liveSession previewView]];
    });
    
}

-(void)initRecord{
    self.pushUrl = @"rtmp://192.168.20.178:1935/mj17/myStream";
    NSLog(@"initRecord()....%@", self);
    //[UnityGetGLViewController() presentViewController: animated: completion:nil];
    [self setupConfiguration];
    _timer = [NSTimer scheduledTimerWithTimeInterval:1.0 target:self selector:@selector(timeUpdate) userInfo:nil repeats:YES];
}

-(void)startRecord{
    //NSLog(@"startRecord().....");
    NSLog(@"startRecord()....%@", self);
    [self setupLiveSession];
    //[UnityGetGLView()  addSubview: self.view];
}
-(void)stopRecord{
    //停止预览，注意:停止预览后将liveSession置为nil
    [self.liveSession alivcLiveVideoStopPreview];
    [self.liveSession.previewView removeFromSuperview];
    //关闭直播
    [self.liveSession alivcLiveVideoDisconnectServer];
    //销毁直播 session
    self.liveSession = nil;
    /*
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
     */
}
/*
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
*/

@end

IMPL_APP_CONTROLLER_SUBCLASS(MyPlugin)



// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
#if __cplusplus
extern "C" {
#endif
    //-------------------------------------------------------------------------------------------------
    void _initRecord(){
        MyPlugin *objVideoCapture = [MyPlugin sharedInstance];
        [objVideoCapture initRecord];
        //[objVideoCapture showDemoView];
    }
    //-------------------------------------------------------------------------------------------------
    //void _tapRecord (){
    //    MyPlugin *objVideoCapture = [MyPlugin GetSharedInstance];
    //    [objVideoCapture tapRecord];
    //}
    void _stopRecord (){
        MyPlugin *objVideoCapture = [MyPlugin sharedInstance];
        [objVideoCapture stopRecord];
    }
    void _startRecord (){
        //NSLog(@"_startRecord()");
        //UnityPause(true);
        // 跳转到IOS界面，Unity界面暂停
        //[MyPlugin GetSharedInstance].unityIsPaused = YES;
        //[GetAppController() setupIOS];
        //GetAppController().window.rootViewController = GetAppController().vc;
        // GetAppController()获取appController，相当于self
        // UnityGetGLView()获取UnityView，相当于_window
        // 点击按钮后跳转到IOS界面，设置界面为IOS界面
        //GetAppController().window.rootViewController = GetAppController().vc;
        MyPlugin *objVideoCapture = [MyPlugin sharedInstance];
        [objVideoCapture startRecord];
    }
    
    //extern "C" UIViewController *UnityGetGLViewController()
    //{
    //    return GetAppController().rootViewController;
    //}
#if __cplusplus
}
#endif

