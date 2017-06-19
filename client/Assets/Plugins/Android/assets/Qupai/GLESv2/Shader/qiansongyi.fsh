precision mediump float;
varying mediump vec2 vTexCoord;
uniform sampler2D sTexture;
uniform sampler2D inputImageTexture2;
vec4 MTSelectiveColor(vec4 baseColor,float iMin, float iMid, float iMax, vec4 disCMY, float Cv, float Mv, float Yv,int nIndex)
{
    int isDo = 0;
    float iLim;
    if (nIndex == 1)//red
    {
        if ((baseColor.r>baseColor.g)&&(baseColor.r>baseColor.b))
        {
            isDo = 1;
            iLim=(iMax-iMid);
        }
    }
    else if (nIndex == 3 )//blue
    {
        if ((baseColor.b>baseColor.g)&&(baseColor.b>baseColor.r))
        {
            isDo = 1;
            iLim=(iMax-iMid);
        }
    }
    else if (nIndex == 4)//yello
    {
        if ((baseColor.b<baseColor.g)&&(baseColor.b<baseColor.r))
        {
            isDo = 1;
            iLim = (iMid-iMin);
        }
    }
    else if(nIndex == 5)//cyan
    {
        if ((baseColor.r<baseColor.g)&&(baseColor.r<baseColor.b))
        {
            isDo = 1;
            iLim=iMid-iMin;
        }
    }
    else if (nIndex == 7) //white
    {
        if ((baseColor.r>0.5)&&(baseColor.g>0.5)&&(baseColor.b>0.5))
        {
            isDo = 1;
            iLim=(iMin-0.5)*2.0;
        }
    }
    if (isDo == 1)
    {
        float nTotal = iLim;
        //follow is same
        float iInc;float iDec;float iValue;
        if (Cv!=0.0)
        {
            iInc=(iLim*baseColor.r);
            iDec=nTotal-iInc;
            if (baseColor.r>0.5)
                iInc=iDec;
            iValue=Cv>0.0?(iInc*Cv):(iDec*Cv);
            disCMY.r += iValue;
        }
        if (Mv!=0.0)
        {
            iInc=(iLim*baseColor.g);
            iDec=nTotal-iInc;
            if (baseColor.g>0.5)
                iInc=iDec;
            iValue=Mv>0.0?(iInc*Mv):(iDec*Mv);
            disCMY.g += iValue;
        }
        if (Yv!=0.0)
        {
            iInc=iLim*baseColor.b;
            iDec=nTotal-iInc;
            if (baseColor.b>0.5)
                iInc=iDec;
            iValue=Yv>0.0?(iInc*Yv):(iDec*Yv);
            disCMY.b += iValue;
        }
    }
    return disCMY;
}

vec4 ABaoColor(vec4 oral)
{
    float fL;float fA;float fBLab;
    float fR = oral.r;float fG = oral.g;float fB = oral.b;
    float fX = 0.431 * fR + 0.342 * fG + 0.178 * fB;
    float fY = 0.222 * fR + 0.707 * fG + 0.071 * fB;
    float fZ = 0.020 * fR + 0.130 * fG + 0.939 * fB;
    float tx = fX / 0.951;float ty = fY;float tz = fZ / 1.089;
    float fTx;float fTy;float fTz;float fLight;
    if (ty > 0.008856)
    {
        fTy = pow(ty, 0.333333);
        fLight = 116.0 * fTy - 16.0;
    }
    else
    {
        fTy = 7.78 * ty + 0.137931;
        fLight = 903.3 * ty;
    }
    fTx = (tx > 0.008856) ? pow(tx, 0.333333) : 7.78 * tx + 0.137931;
    fTz = (tz > 0.008856) ? pow(tz, 0.333333) : 7.78 * tz + 0.137931;
    fL = fLight * 1.0038;
    fA = (fTx - fTy) * 500.0 ;
    fBLab = fA;
    float fHa;float fHb;float fSqyyn;
    float fP = (fL + 16.0) / 116.0;
    float fYyn = fP * fP * fP;
    if (fYyn > 0.008856)
    {
        fY = fYyn;
        fHa = (fP + fA / 500.0);

        fX = 0.951 * fHa * fHa * fHa;
        fHb = (fP - fBLab / 200.0);
        fZ = 1.089 * fHb * fHb * fHb;
    }
    else
    {
        fY = fL / 903.3;
        fSqyyn = pow(fL / 903.3, 0.333333);
        fHa = fA / 500.0 / 7.787 + fSqyyn;

        fX = 0.951 * fHa * fHa * fHa;
        fHb = fSqyyn - fBLab /200. / 7.787;
        fZ = 1.089 * fHb * fHb * fHb;
    }
    fR =  3.063 * fX - 1.393 * fY - 0.476 * fZ;
    fG = -0.969 * fX + 1.876 * fY + 0.042 * fZ;
    fB  =  0.068 * fX - 0.229 * fY + 1.069 * fZ;
    fR = max(0.0, min(1.0, fR));
    fG = max(0.0, min(1.0, fG));
    fB = max(0.0, min(1.0, fB));
    oral.r = fR;
    oral.g = fG;
    oral.b = fB;
    oral.r = texture2D( inputImageTexture2, vec2(oral.r,0.5)).r;
    oral.g = texture2D( inputImageTexture2, vec2(oral.g,0.5)).g;
    oral.b = texture2D( inputImageTexture2, vec2(oral.b,0.5)).b;
    mediump vec4 disCMY;
    mediump vec4 baseColor;
    mediump float iMin;
    mediump float iMid;
    mediump float iMax;
    mediump float isRelative;
    baseColor= oral;
    disCMY=vec4(0.0);
    iMin=min(baseColor.r,min(baseColor.g,baseColor.b));
    iMax=max(baseColor.r,max(baseColor.g,baseColor.b));
    iMid=iMin;
    if ((baseColor.r>iMin)&&(baseColor.r<iMax)) iMid=baseColor.r;
    else
        if ((baseColor.g>iMin)&&(baseColor.g<iMax)) iMid=baseColor.g;
        else
            if ((baseColor.b>iMin)&&(baseColor.b<iMax)) iMid=baseColor.b;   disCMY=MTSelectiveColor(baseColor,iMin,iMid,iMax,disCMY,0.0,0.0,0.5,5);  disCMY=MTSelectiveColor(baseColor,iMin,iMid,iMax,disCMY,0.0,-1.0,1.0,3);

    oral.r=clamp(baseColor.r-disCMY.x,0.0,1.0);
    oral.g=clamp(baseColor.g-disCMY.y,0.0,1.0);
    oral.b=clamp(baseColor.b-disCMY.z,0.0,1.0);
    return oral;
}

void main()
{
    mediump vec4 oralData =texture2D(sTexture, vTexCoord);
    oralData = ABaoColor(oralData);
    gl_FragColor = oralData;
}
