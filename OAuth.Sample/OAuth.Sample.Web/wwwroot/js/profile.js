var token, userId;

window.onload = function () {
    token = Cookies.get("JwtToken", { path: window.location.pathname });
    userId = Cookies.get("UserId", { path: window.location.pathname });

    let url = new URL(this.location.href);
   
    if (url.searchParams.has('code') && url.searchParams.has('state')) {
        let state = url.searchParams.get('state');
        if (state != Cookies.get("LineNotifyState", { path: window.location.pathname })) {
            // TODO: Cross Site
            return;
        }

        let stateArr = state.split('_');
        switch (stateArr[0]) {
            case 'LineNotify':
                SubscribeLineNotify(userId, url.searchParams.get('code'));
            break;
        }
        return;
    }

    if (userId) {
        GetUserData(userId);
        return;
    }

}

var GetUserData = function(userId) {
    let getUrl = `${$("#BaseDomainApiUrl").val()}/User/${userId}`;
    $.ajax({
        method: "GET",
        url: getUrl,
        dataType: "json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        success: function (result) {
            console.log('success', result);

            $('#Name').text(result.Name);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}

var UserLineNotifyAuth = function () {
    let state = `LineNotify_${Date.now()}`;
    Cookies.set("LineNotifyState", state, { path: this.location.pathname });

    let authUrl = 'https://notify-bot.line.me/oauth/authorize?';
    authUrl += 'response_type=code';
    authUrl += `&client_id=JXHlpy3ASuitjhpFEYeymW`;
    authUrl += `&redirect_uri=https://localhost:44350/Profile`;
    authUrl += `&state=${state}`;
    authUrl += '&scope=notify';
    window.location.href = authUrl; 
}

var SubscribeLineNotify = function (userId, code) {
    let postUrl = `${$("#BaseDomainApiUrl").val()}/LineNotify/Subscribe`;
    let postData = { UserId: userId, Code: code };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}

var UnSubscribeLineNotify = function () {
    let url = new URL(this.location.href);
    let postUrl = `${$("#BaseDomainApiUrl").val()}/LineNotify/UnSubscribe`;
    let postData = { UserId: userId };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}

var SendMessage = function() {
    let postUrl = `${$("#BaseDomainApiUrl").val()}/LineNotify/Notify`;
    let postData = { Message: 'Hello World!' };

    $.ajax({
        method: "POST",
        url: postUrl,
        contentType: "application/json",
        headers: {
            'Authorization': `Bearer ${token}`
        },
        data: JSON.stringify(postData),
        dataType: "json",
        success: function (result) {
            console.log('success', result);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}