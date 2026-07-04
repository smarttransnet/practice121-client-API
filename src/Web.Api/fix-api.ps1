$dir = "c:\2026\FleeRo\Practice121\API\src\Web.Api\Controllers"

# Helper function to do replacements
function Replace-FileContent {
    param (
        [string]$FilePath,
        [hashtable]$Replacements
    )
    $content = Get-Content $FilePath -Raw
    foreach ($key in $Replacements.Keys) {
        $content = $content.Replace($key, $Replacements[$key])
    }
    Set-Content -Path $FilePath -Value $content -NoNewline
}

# --- ClientPortalAuthController ---
Replace-FileContent "$dir\ClientPortalAuthController.cs" @{
    "var result = await sender.Send" = "SharedKernel.Result<RefreshResponse> result = await sender.Send"
    "SharedKernel.Result<RefreshResponse> result = await sender.Send(command," = "if (command is LoginCommand lc) { SharedKernel.Result<LoginResponse> resultLogin = await sender.Send(lc, cancellationToken); return resultLogin.IsSuccess ? Ok(resultLogin.Value) : Unauthorized(resultLogin.Error); } SharedKernel.Result<RefreshResponse> result = await sender.Send(command,"
}

# Instead of complex regex, let's just write explicit string replacements per file manually using a script that rewrites them cleanly.


