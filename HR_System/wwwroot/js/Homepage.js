$(document).ready(function () {

    $("#SignUpForm").fadeOut("fast");

    $("#createAccountLink").click(function () {

        $("#SignInForm").fadeOut("fast", function () {
            $("#logo").fadeOut("fast", function () {
                $("#circle").animate({
                    height: '0px',
                    width: '0px',
                    left: '-20%',
                    bottom: '592px'
                }, "slow"
                    , function () {
                        $(this).animate({
                            left: '-20%',
                            bottom: '592px',
                            height: '1000px',
                            width: '72%'
                        }, "slow", function () {
                            $("#SignUpForm").fadeIn("fast");
                            $("#logo").attr('src', '../images/whiteLogo.png');
                            $("#logo").fadeIn("fast");
                        });
                    });
            });
        });
    });

    $("#goToSignInLink").click(function () {

        $("#SignUpForm").fadeOut("fast", function () {
            $("#logo").fadeOut("fast", function () {
                $("#circle").animate({
                    height: '0px',
                    width: '0px',
                    left: '100%',
                    bottom: '515px'
                }, "slow"
                    , function () {
                        $(this).animate({
                            left: '49%',
                            bottom: '515px',
                            height: '1000px',
                            width: '72%'
                        }, "slow", function () {
                            $("#SignInForm").fadeIn("fast");
                            $("#logo").attr('src', '../images/purpleLogo.png');
                            $("#logo").fadeIn("fast");
                        });
                    });
            });
        });       
    });    
});