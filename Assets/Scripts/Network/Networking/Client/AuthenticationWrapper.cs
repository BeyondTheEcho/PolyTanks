using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState s_AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuthAsync(int maxTries = 5)
    {
        if (s_AuthState == AuthState.Authenticated) return s_AuthState;

        if (s_AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating");
            await Authenticating();
            return s_AuthState;
        }

        await SignInAnonymouslyAsync(maxTries);

        return s_AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (s_AuthState == AuthState.Authenticating || s_AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return s_AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries)
    {
        s_AuthState = AuthState.Authenticating;

        int tries = 0;

        while (s_AuthState == AuthState.Authenticating && tries < maxTries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    s_AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException authenticationEx)
            {
                Debug.LogError(authenticationEx);
                s_AuthState = AuthState.Error;
            }
            catch (RequestFailedException requestFailedEx)
            {
                Debug.LogError(requestFailedEx);
                s_AuthState = AuthState.Error;
            }

            tries++;

            await Task.Delay(1000);
        }

        if (s_AuthState != AuthState.Authenticated)
        {
            Debug.LogWarning($"Player not signed it successfully with {maxTries} attempts");
            s_AuthState = AuthState.TimeOut;
        }
    }
}

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}