window.onload = function () {
    let url = new URL(this.location.href);

    if (url.searchParams.has('code') && url.searchParams.has('state')) {
        let state = url.searchParams.get('state');
        if (state != Cookies.get("LoginState", { path: this.location.pathname })) {
            bootbox.alert({
                message: "state驗證失敗"
            });
            return;
        }

        GetUserData(state.split('_')[0], url.searchParams.get("code"));
    } else {
        RemoveLoginCookie();
        $('#Loading').hide();
    }
}

var LineLogin = function () {
    let state = `LineLogin_${Date.now()}`;
    Cookies.set("LoginState", state, { path: this.location.pathname });

    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == 'LineLogin');
    let url = 'https://access.line.me/oauth2/v2.1/authorize?';
    url += 'response_type=code';
    url += `&client_id=${oauthSetting.ClientId}`;
    url += `&redirect_uri=${oauthSetting.RedirectUri}`;
    url += `&state=${state}`;
    url += '&scope=profile%20openid%20email';
    window.location.href = url;
}

var FacebookLogin = function () {
    let state = `FacebookLogin_${Date.now()}`;
    Cookies.set("LoginState", state, { path: this.location.pathname });

    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == 'FacebookLogin');
    let url = 'https://www.facebook.com/v13.0/dialog/oauth?';
    url += `&client_id=${oauthSetting.ClientId}`;
    url += `&redirect_uri=${oauthSetting.RedirectUri}`;
    url += `&state=${state}`;
    window.location.href = url;
}

var GoogleLogin = function () {
    let state = `GoogleLogin_${Date.now()}`;
    Cookies.set("LoginState", state, { path: this.location.pathname });

    let oauthSetting = settings.OAuthSettings.find(x => x.ProviderType == 'GoogleLogin');
    let url = 'https://accounts.google.com/o/oauth2/v2/auth?';
    url += 'response_type=code';
    url += `&client_id=${oauthSetting.ClientId}`;
    url += `&redirect_uri=${oauthSetting.RedirectUri}`;
    url += `&state=${state}`;
    url += '&scope=https://www.googleapis.com/auth/userinfo.profile';
    window.location.href = url;
}

var GetUserData = function (providerType, code) {
    let postUrl = `${settings.BaseDomainApiUrl}/Login/OAuthLogin`;
    let postData = { ProviderType: providerType, Code: code };

    $('#Loading').show();
    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            SetLoginCookie(result);
            window.location.href = '../Profile';
            $('#Loading').hide();
        },
        error: function (result) {
            bootbox.alert({
                message: "OAuth登入失敗"
            });

            $('#Loading').hide();
        }
    });
}

function SetLoginCookie(loginData) {
    Cookies.set("UserId", loginData.UserId);
    Cookies.set("JwtToken", loginData.JwtToken); 
}

function RemoveLoginCookie() {
    Cookies.remove('UserId');
    Cookies.remove('JwtToken');
    Cookies.remove('LoginState');
}
