

window.onload = function() {
    let url = new URL(this.location.href);

    if (url.searchParams.has('code') && url.searchParams.has('state')) {
        UserLogin(url.searchParams.get("code"));
    }
}

var OAuthLogin = function () {
    let url = 'https://access.line.me/oauth2/v2.1/authorize?';
    url += 'response_type=code';
    url += `&client_id=1654549010`;
    url += `&redirect_uri=https://localhost:44350`;
    url += '&state=123';
    url += '&scope=profile%20openid%20email';
    window.location.href = url;
}

var UserLogin = function(code) {
    let postUrl = `${$("#BaseDomainApiUrl").val()}/Login/OAuthLogin`;
    let postData = { ProviderType: 'LineLogin', Code: code };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);

            window.location.href = `../Profile?userId=${result.UserId}`;
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}
