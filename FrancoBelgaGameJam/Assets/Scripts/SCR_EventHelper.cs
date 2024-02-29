using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SCR_EventHelper
{
    public static bool TryUnSubFromEventHandler(EventHandler eventHandler)
    {
        try
        {
            if (eventHandler == null) return false;
            
            var invocationList = eventHandler.GetInvocationList();
            for (int i = invocationList.Length - 1; i >= 0; i--)
            {
                eventHandler -= invocationList[i] as EventHandler;
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
    
    public static bool TryUnSubFromEventHandler<T>(EventHandler<T> eventHandler)
    {
        try
        {
            if (eventHandler == null) return false;
            
            var invocationList = eventHandler.GetInvocationList();
            for (int i = invocationList.Length - 1; i >= 0; i--)
            {
                eventHandler -= invocationList[i] as EventHandler<T>;
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
    public static bool TrySendEvent(EventHandler eventHandler, object sender)
    {
        try
        {
            var handler = eventHandler;
            handler?.Invoke(sender, null);
            return true;
        }
        catch(Exception e)
        {
            // Try making a more descriptive error message
            Debug.LogError(e);
            return false;
        }
    }
    public static bool TrySendEvent<T>(EventHandler<T> eventHandler, object sender, T args)
    where T : EventArgs
    {
        try
        {
            var handler = eventHandler;
            handler?.Invoke(sender, args);
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
}
