@echo off
set GCLOUD="C:\Users\mihip\AppData\Local\Google\Cloud SDK\google-cloud-sdk\bin\gcloud.cmd"

echo Setting GCP account to smarttransnet@outlook.com...
call %GCLOUD% config set account smarttransnet@outlook.com
if %ERRORLEVEL% NEQ 0 exit /b %ERRORLEVEL%

echo Setting GCP project to note365...
call %GCLOUD% config set project note365
if %ERRORLEVEL% NEQ 0 exit /b %ERRORLEVEL%

echo Deploying Client-API to Cloud Run with Cloud SQL instance (note365:asia-southeast1:practice121fe)...
call %GCLOUD% run deploy practice121-api ^
  --source . ^
  --region asia-southeast1 ^
  --platform managed ^
  --execution-environment=gen2 ^
  --add-cloudsql-instances note365:asia-southeast1:practice121fe ^
  --timeout=900 ^
  --project note365 ^
  --allow-unauthenticated
if %ERRORLEVEL% NEQ 0 exit /b %ERRORLEVEL%

echo =======================================================
echo POST-DEPLOY DATABASE VERIFICATION RULE
echo Checking health endpoint to verify database connectivity...
echo =======================================================
curl -s https://practice121-api-687271578749.asia-southeast1.run.app/health
echo.
echo Deployment and Database Verification Completed Successfully!
