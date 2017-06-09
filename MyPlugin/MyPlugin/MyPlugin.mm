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
    [_sharedInstance startPlay];
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

- (void)timeUpdate {
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
}

-(void)startPlay{
    NSLog(@"startPlay()....%@", self);
    //初始化播放器的类
    player = [[AliVcMediaPlayer alloc] init];
    //创建播放器，传入显示窗口
    [player create:mShowView];
    //注册准备完成通知
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(OnVideoPrepared:) name:AliVcMediaPlayerLoadDidPreparedNotification object:player];
    //注册错误通知
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(OnVideoError:) name:AliVcMediaPlayerPlaybackErrorNotification object:player];
    //传入播放地址，准备播放
    [player prepareToPlay:mUrl];
    //开始播放
    [player play];
}

-(void)stopPlay{
    NSLog(@"stopPlay()....%@", self);
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(MyPlugin)



// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
extern "C" {
    //-------------------------------------------------------------------------------------------------
    void _initRecord(){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj initRecord];
    }
    void _stopRecord (){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj stopRecord];
    }
    void _startRecord (){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj startRecord];
    }
    void _stopPlay (){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj stopPlay];
    }
    void _startPlay (){
        MyPlugin *obj = [MyPlugin sharedInstance];
        [obj startPlay];
    }
}
