using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState s_AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuthAsync(int maxTries = 5)
    {
        if (s_AuthState == AuthState.Authenticated) return s_AuthState;

        s_AuthState = AuthState.Authenticating;

        int tries = 0;
        while(s_AuthState == AuthState.Authenticating && tries < maxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                s_AuthState = AuthState.Authenticated;
                break;
            }

            tries++;

            await Task.Delay(1000);
        }

        return s_AuthState;
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