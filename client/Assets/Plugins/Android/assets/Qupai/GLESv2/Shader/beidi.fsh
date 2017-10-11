precision mediump float;
varying mediump vec2 vTexCoord;

uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;

const mediump vec3 clColor1=vec3(0.0235,0.0706,0.196);

#define BlendExclusion(base, blend)     (base + blend - 2.0 * base * blend)

    //Selective Color Red
    mediump vec3 getSelectiveColorRed(mediump vec3 baseColor,mediump float iMin,mediump float iMid,mediump float iMax,mediump vec3 disCMY, mediump float Cv,mediump float Mv,mediump float Yv,mediump float Bv,mediump float isRelative)
{
    mediump float r;
    mediump float g;
    mediump float b;
    mediump vec3 result=disCMY;
    if ((baseColor.r>baseColor.g)&&(baseColor.r>baseColor.b))
    {
        mediump float iLim=iMax-iMid;

        //follow is same
        mediump float iInc;
        mediump float iDec;
        mediump float iValue;
        mediump float disC=disCMY.x;
        mediump float disM=disCMY.y;
        mediump float disY=disCMY.z;

        if (Cv!=0.0)
        {
            iInc=(iLim*baseColor.r);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.r>0.5))
                iInc=iDec;
            iValue=Cv>0.0?(iInc*Cv):(iDec*Cv);
            disC=disC+iValue;
        }

        if (Mv!=0.0)
        {
            iInc=(iLim*baseColor.g);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.g>0.5))
                iInc=iDec;
            iValue=Mv>0.0?(iInc*Mv):(iDec*Mv);
            disM=disM+iValue;
        }

        if (Yv!=0.0)
        {
            iInc=iLim*baseColor.b;
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.b>0.5))
                iInc=iDec;
            iValue=Yv>0.0?(iInc*Yv):(iDec*Yv);
            disY=disY+iValue;
        }

        result=vec3(disC,disM,disY);
    }

    return result;
}

    //Selective Color Yellow
    mediump vec3 getSelectiveColorYellow(mediump vec3 baseColor,mediump float iMin,mediump float iMid,mediump float iMax,mediump vec3 disCMY, mediump float Cv,mediump float Mv,mediump float Yv,mediump float Bv,mediump float isRelative)
{
    mediump float r;
    mediump float g;
    mediump float b;
    mediump vec3 result=disCMY;
    if ((baseColor.b<baseColor.g)&&(baseColor.b<baseColor.r))
    {
        mediump float iLim=iMid-iMin;

        //follow is same
        mediump float iInc;
        mediump float iDec;
        mediump float iValue;
        mediump float disC=disCMY.x;
        mediump float disM=disCMY.y;
        mediump float disY=disCMY.z;

        if (Cv!=0.0)
        {
            iInc=(iLim*baseColor.r);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.r>0.5))
                iInc=iDec;
            iValue=Cv>0.0?(iInc*Cv):(iDec*Cv);
            disC=disC+iValue;
        }

        if (Mv!=0.0)
        {
            iInc=(iLim*baseColor.g);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.g>0.5))
                iInc=iDec;
            iValue=Mv>0.0?(iInc*Mv):(iDec*Mv);
            disM=disM+iValue;
        }

        if (Yv!=0.0)
        {
            iInc=iLim*baseColor.b;
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.b>0.5))
                iInc=iDec;
            iValue=Yv>0.0?(iInc*Yv):(iDec*Yv);
            disY=disY+iValue;
        }

        result=vec3(disC,disM,disY);
    }

    return result;
}

    //8.Selective Color Gray
    mediump vec3 getSelectiveColorGray(mediump vec3 baseColor,mediump float iMin,mediump float iMid,mediump float iMax,mediump vec3 disCMY, mediump float Cv,mediump float Mv,mediump float Yv,mediump float Bv,mediump float isRelative)
{
    mediump float r;
    mediump float g;
    mediump float b;
    mediump vec3 result=disCMY;

    if (((baseColor.r==0.0)&&(baseColor.g==0.0)&&(baseColor.b==0.0)||(baseColor.r==1.0)&&(baseColor.g==1.0)&&(baseColor.b==1.0)))
    {
        mediump float iLim=abs(1.0-(abs(iMax-0.5)+abs(iMin-0.5)));

        //follow is same
        mediump float iInc;
        mediump float iDec;
        mediump float iValue;
        mediump float disC=disCMY.x;
        mediump float disM=disCMY.y;
        mediump float disY=disCMY.z;

        if (Cv!=0.0)
        {
            iInc=(iLim*baseColor.r);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.r>0.5))
                iInc=iDec;
            iValue=Cv>0.0?(iInc*Cv):(iDec*Cv);
            disC=disC+iValue;
        }

        if (Mv!=0.0)
        {
            iInc=(iLim*baseColor.g);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.g>0.5))
                iInc=iDec;
            iValue=Mv>0.0?(iInc*Mv):(iDec*Mv);
            disM=disM+iValue;
        }

        if (Yv!=0.0)
        {
            iInc=iLim*baseColor.b;
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.b>0.5))
                iInc=iDec;
            iValue=Yv>0.0?(iInc*Yv):(iDec*Yv);
            disY=disY+iValue;
        }

        result=vec3(disC,disM,disY);
    }

    return result;
}

    //9.Selective Color Black
    mediump vec3 getSelectiveColorBlack(mediump vec3 baseColor,mediump float iMin,mediump float iMid,mediump float iMax,mediump vec3 disCMY, mediump float Cv,mediump float Mv,mediump float Yv,mediump float Bv,mediump float isRelative)
{
    mediump float r;
    mediump float g;
    mediump float b;
    mediump vec3 result=disCMY;

    if ((baseColor.r<0.5)&&(baseColor.g<0.5)&&(baseColor.b<0.5))
    {
        mediump float iLim=(0.5-iMax)*2.0;

        //follow is same
        mediump float iInc;
        mediump float iDec;
        mediump float iValue;
        mediump float disC=disCMY.x;
        mediump float disM=disCMY.y;
        mediump float disY=disCMY.z;

        if (Cv!=0.0)
        {
            iInc=(iLim*baseColor.r);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.r>0.5))
                iInc=iDec;
            iValue=Cv>0.0?(iInc*Cv):(iDec*Cv);
            disC=disC+iValue;
        }

        if (Mv!=0.0)
        {
            iInc=(iLim*baseColor.g);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.g>0.5))
                iInc=iDec;
            iValue=Mv>0.0?(iInc*Mv):(iDec*Mv);
            disM=disM+iValue;
        }

        if (Yv!=0.0)
        {
            iInc=iLim*baseColor.b;
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.b>0.5))
                iInc=iDec;
            iValue=Yv>0.0?(iInc*Yv):(iDec*Yv);
            disY=disY+iValue;
        }

        result=vec3(disC,disM,disY);
    }

    return result;
}

    //5.Selective Color blue
    mediump vec3 getSelectiveColorBlue(mediump vec3 baseColor,mediump float iMin,mediump float iMid,mediump float iMax,mediump vec3 disCMY, mediump float Cv,mediump float Mv,mediump float Yv,mediump float Bv,mediump float isRelative)
{
    mediump float r;
    mediump float g;
    mediump float b;
    mediump vec3 result=disCMY;

    if ((baseColor.b>baseColor.g)&&(baseColor.b>baseColor.r))
    {
        mediump float iLim=iMax-iMid;

        //follow is same
        mediump float iInc;
        mediump float iDec;
        mediump float iValue;
        mediump float disC=disCMY.x;
        mediump float disM=disCMY.y;
        mediump float disY=disCMY.z;

        if (Cv!=0.0)
        {
            iInc=(iLim*baseColor.r);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.r>0.5))
                iInc=iDec;
            iValue=Cv>0.0?(iInc*Cv):(iDec*Cv);
            disC=disC+iValue;
        }

        if (Mv!=0.0)
        {
            iInc=(iLim*baseColor.g);
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.g>0.5))
                iInc=iDec;
            iValue=Mv>0.0?(iInc*Mv):(iDec*Mv);
            disM=disM+iValue;
        }

        if (Yv!=0.0)
        {
            iInc=iLim*baseColor.b;
            iDec=iLim-iInc;
            if ((isRelative>0.0)&&(baseColor.b>0.5))
                iInc=iDec;
            iValue=Yv>0.0?(iInc*Yv):(iDec*Yv);
            disY=disY+iValue;
        }

        result=vec3(disC,disM,disY);
    }

    return result;
}

    void main()
{
    mediump vec3 clB;
    mediump vec3 clC;
    mediump vec3 clD;
    mediump vec3 clE;
    mediump vec3 clA=texture2D(sTexture, vTexCoord).rgb;

    //1.
    clB=BlendExclusion(clA,clColor1);

    //2
    mediump vec3 disCMY;
    mediump vec3 baseColor;
    mediump float iMin;
    mediump float iMid;
    mediump float iMax;
    mediump float isRelative;
    baseColor=clB;
    disCMY=vec3(0.0,0.0,0.0);
    isRelative=1.0;   //true--1.0   false--0.0
    iMin=min(baseColor.r,min(baseColor.g,baseColor.b));
    iMax=max(baseColor.r,max(baseColor.g,baseColor.b));
    iMid=iMin;
    if ((baseColor.r>iMin)&&(baseColor.r<iMax)) iMid=baseColor.r;
    else
        if ((baseColor.g>iMin)&&(baseColor.g<iMax)) iMid=baseColor.g;
        else
            if ((baseColor.b>iMin)&&(baseColor.b<iMax)) iMid=baseColor.b;

    disCMY=getSelectiveColorRed(baseColor,iMin,iMid,iMax,disCMY,-1.0,-0.6,-0.6,0.0,isRelative);
    disCMY=getSelectiveColorYellow(baseColor,iMin,iMid,iMax,disCMY,-0.    ,-0.62,-1.0,0.0,isRelative);
    disCMY=getSelectiveColorBlue(baseColor,iMin,iMid,iMax,disCMY,-0.15,1.0,1.0,0.0,isRelative);
    disCMY=getSelectiveColorGray(baseColor,iMin,iMid,iMax,disCMY,-0.12,-0.06,0.0,0.0,isRelative);
    disCMY=getSelectiveColorBlack(baseColor,iMin,iMid,iMax,disCMY,-0. ,0.04,-0.05,0.0,isRelative);

    clC.r=clamp(baseColor.r-disCMY.x,0.0,1.0);
    clC.g=clamp(baseColor.g-disCMY.y,0.0,1.0);
    clC.b=clamp(baseColor.b-disCMY.z,0.0,1.0);

    //3
    clD.r=texture2D(inputImageTexture2,vec2(clC.r,0.5)).r;
    clD.g=texture2D(inputImageTexture2,vec2(clC.g,0.5)).g;
    clD.b=texture2D(inputImageTexture2,vec2(clC.b,0.5)).b;

    gl_FragColor=vec4(clD,1.0);
}