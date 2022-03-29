

window.onload = function() {
    let url = new URL(this.location.href);

    if (url.searchParams.has('code') && url.searchParams.has('state')) {
        let state = url.searchParams.get('state');
        if (state != Cookies.get("LoginState", { path: this.location.pathname })) {
            // TODO: Cross Site
            return;
        }

        GetUserData(state.split('_')[0], url.searchParams.get("code"));
    } else {
        RemoveLoginCookie();
    }
}

var LineLogin = function () {
    let state = `LineLogin_${Date.now()}`;
    Cookies.set("LoginState", state, { path: this.location.pathname });

    let url = 'https://access.line.me/oauth2/v2.1/authorize?';
    url += 'response_type=code';
    url += `&client_id=1654549010`;
    url += `&redirect_uri=https://localhost:44350`;
    url += `&state=${state}`;
    url += '&scope=profile%20openid%20email';
    window.location.href = url;
}

var FacebookLogin = function () {
    let state = `FacebookLogin_${Date.now()}`;
    Cookies.set("LoginState", state, { path: this.location.pathname });

    let url = 'https://www.facebook.com/v13.0/dialog/oauth?';
    url += `&client_id=389331466526369`;
    url += `&redirect_uri=https://localhost:44350`;
    url += `&state=${state}`;
    window.location.href = url;
}

var GoogleLogin = function () {
    let state = `GoogleLogin_${Date.now()}`;
    Cookies.set("LoginState", state, { path: this.location.pathname });

    let url = 'https://accounts.google.com/o/oauth2/v2/auth?';
    url += 'response_type=code';
    url += `&client_id=632085700226-u2g1r5va353ib8t3fkd14kq92oh80367.apps.googleusercontent.com`;
    url += `&redirect_uri=https://localhost:44350`;
    url += `&state=${state}`;
    url += '&scope=https://www.googleapis.com/auth/userinfo.profile';
    window.location.href = url;
}

var GetUserData = function (providerType, code) {
    let postUrl = `${$("#BaseDomainApiUrl").val()}/Login/OAuthLogin`;
    let postData = { ProviderType: providerType, Code: code };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);

            SetLoginCookie(result);
            window.location.href = '../Profile';
        },
        error: function (result) {
            console.log('error', result);
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
