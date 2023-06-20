(function() {
    $(document).ready(function () {
        $('#fetchNumber').click(async function () {
            console.log("Button fetchNumber clicked");

            const response = await fetch("api/static/RetreiveUnsentCount");
            const data = await response.json();
            console.log("Fetch success");
            document.getElementById('unsentNumber').value = data.number;
        });

        $('#notifPost').click(function () {
            $.ajax({
                type: "POST",
                url: "/api/static/SendAll",
                success: function () {
                    document.getElementById('unsentNumber').value = 0;
                    console.log("Post success");
                },
                error: function () {
                    console.log("Post error");
                }
            });

        });

    });
})();
