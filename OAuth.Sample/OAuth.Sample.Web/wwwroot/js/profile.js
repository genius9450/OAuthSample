window.onload = function () {
    let url = new URL(this.location.href);
    if (url.searchParams.has('userId') ){
        UserLogin(url.searchParams.get("userId"));
    }
}

var UserLogin = function(userId) {
    let getUrl = `${$("#BaseDomainApiUrl").val()}/User/${userId}`;
    $.ajax({
        method: "GET",
        url: getUrl,
        dataType: "json",
        success: function (result) {
            console.log('success', result);

            $('#Name').text(result.Name);
        },
        error: function (result) {
            console.log('error', result);
        }
    });
}