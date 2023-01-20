using Godot;

public class Login : MarginContainer
{
    private Window window;

    private HttpClient httpClient;

    [Export]
    private NodePath lineEditPath;

    private LineEdit lineEdit;

    [Export]
    private NodePath buttonPath;

    private Button button;

    public override void _Ready()
    {
        window = AutoLoad.Of(this).Window;

        httpClient = AutoLoad.Of(this).HttpClient;

        lineEdit = GetNode<LineEdit>(lineEditPath);

        lineEdit.Connect("text_changed", this, nameof(OnUsernameChanged));

        button = GetNode<Button>(buttonPath);

        button.Connect("pressed", this, nameof(OnButtonPressed));

        button.Disabled = true;
    }

    private bool isValidUsername(string value)
    {
        var regex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-zA-Z가-힣])[a-zA-Z가-힣]{2,8}$");

        return regex.IsMatch(value);
    }

    private void OnUsernameChanged(string value)
    {
        button.Disabled = !isValidUsername(value);
    }

    private void OnButtonPressed()
    {
        button.Disabled = true;

        new System.Threading.Thread(async () =>
        {
            var uri = new Uri("https://api.eastonline.kr", "/dev/auth");

            var body = Serializer.ToJson(new AuthRequestBody
            {
                username = lineEdit.Text
            });

            var result = await httpClient.PostAsync(uri, body);

            if (result is Ok<HttpClient.Response> ok)
            {
                if (ok.value.statusCode > 500)
                {
                    window.PopupDialog("예기치 못한 오류입니다.");

                    button.Disabled = false;

                    return;
                }

                if (ok.value.statusCode != 200)
                {
                    var json = Deserializer.FromJson<ExceptionResponse>(ok.value.text);

                    window.PopupDialog(json.message);

                    button.Disabled = false;

                    return;
                }

                {
                    var json = Deserializer.FromJson<AuthResponse>(ok.value.text);

                    window.PopupDialog(json.token);
                }
            }
        }).Start();
    }

    public override void _ExitTree()
    {
        lineEdit.Disconnect("text_changed", this, nameof(OnUsernameChanged));

        button.Disconnect("pressed", this, nameof(OnButtonPressed));
    }
}
