package com.unity.oculus;

import android.app.Activity;
import android.view.Surface;
import android.view.SurfaceView;
import android.view.SurfaceHolder;
import android.util.Log;
import android.view.ViewGroup;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Build;
import java.util.Locale;
import com.unity3d.player.UnityPlayer;

public class OculusUnity implements SurfaceHolder.Callback
{
    Activity activity;
    SurfaceView glView;

    public void initOculus()
    {
        Log.d("Unity", "initOculus Java!");
        activity = UnityPlayer.currentActivity;

        activity.runOnUiThread(() -> {
            glView = null;
            int surfaceViewId = activity.getResources().getIdentifier("unitySurfaceView", "id", activity.getPackageName());

            if(surfaceViewId == 0){
                Log.e("Unity", "Failed to find SurfaceView Identifier!");
            }
            else{
                SurfaceView view = activity.findViewById(surfaceViewId);

                if(view != null && view instanceof SurfaceView){
                    glView = view;
                    glView.getHolder().addCallback(this);
                }
            }

            if (glView == null) {
                Log.e("Unity", "Failed to find GlView!");
            }

            Log.d("Unity", "Oculus UI thread done.");

            surfaceCreated(glView.getHolder().getSurface());
        });
    }

    public void pauseOculus()
    {
    }

    public void resumeOculus()
    {
    }

    public void destroyOculus()
    {
        if (glView != null)
            glView.getHolder().removeCallback(this);
    }

    private native void surfaceCreated(Surface glView);

    public static void loadLibrary(String name) {
        Log.d("Unity", "loading library " + name);
        java.lang.System.loadLibrary(name);
    }

    public static boolean getManifestSetting(String boolName) {
        boolean ret = false;

        try
        {
            Activity activity = UnityPlayer.currentActivity;
            ApplicationInfo appInfo = activity.getPackageManager().getApplicationInfo(activity.getPackageName(), PackageManager.GET_META_DATA);
            Bundle bundle = appInfo.metaData;
            ret = bundle.getBoolean(boolName);
        }
        catch (Exception e)
        {
            Log.d("Unity", "Oculus XR Plugin init error");
        }

        return ret;
    }

    public static boolean getLateLatching() {
        return getManifestSetting("com.unity.xr.oculus.LateLatching");
    }

    public static boolean getLateLatchingDebug() {
        return getManifestSetting("com.unity.xr.oculus.LateLatchingDebug");
    }

    public static boolean getLowOverheadMode() {
        return getManifestSetting("com.unity.xr.oculus.LowOverheadMode");
    }

    public static boolean getIsOnOculusHardware() {
        String manufacturer = android.os.Build.MANUFACTURER;
        return manufacturer.toLowerCase(Locale.ENGLISH).contains("oculus");
    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        surfaceCreated(holder.getSurface());
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int format, int w, int h) {
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
    }
}
