$(Document).ready(function() {
	$("#login").click(function() {
		var email = $("#inputEmail").val();
		var password = $("#inputPassword").val();
		$("#returnmessage").empty();
		if (email == '' || password == '') {
			alert("Please fill the required fields.")
		}
		else
		{
			resourceCall("login", email, pass);
		}
	}
}