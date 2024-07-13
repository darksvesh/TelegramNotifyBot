# Telegram Notification Bot

This C# application sends notifications to Telegram users by receiving HTTP requests containing encrypted or unencrypted messages. 
The bot supports encryption based on environment variables. Based on Telegram.Bot library.

## Features

- **Send notifications** to Telegram user(s) via HTTP requests.
- **Supports both encrypted and unencrypted messages**, configurable via environment variable.
- **Docker container support** for easy deployment.
- **Docker healthcheck support**.

## Environment Variables

To configure the bot, set the following environment variables:

| Variable                        | Description                                       |
|---------------------------------|---------------------------------------------------|
| `TELEGRAM_BOT_TOKEN`           | Token for the Telegram bot                        |
| `TELEGRAM_BOT_USE_ENCRYPTION`  | Set to `true` to enable encryption, `false` otherwise |
| `TELEGRAM_BOT_ENCRYPTION_SECRET` | Secret key used for message encryption           |

## JSON Payload Structure

The bot expects incoming HTTP requests to have a JSON payload structured as follows:

```json
{
  "EncryptedChatId": "base64_encoded_encrypted_chat_id",
  "EncryptedMessage": "base64_encoded_encrypted_message",
  "EncryptedChecksum": "base64_encoded_encrypted_checksum",
  "EncryptedDateTime": "base64_encoded_encrypted_unix_timestamp"
}
```
## To run / build with docker
```
git clone https://github.com/darksvesh/TelegramNotifyBot.git
cd telegram-notify-bot
docker build -t telegram-notify-bot .
docker run -e TELEGRAM_BOT_TOKEN=MyValue -e TELEGRAM_BOT_USE_ENCRYPTION=0 -e TELEGRAM_BOT_ENCRYPTION_SECRET="MyValue" telegram-notify-bot:latest
```
## Author notes
I made this bot to send me technical notifications about current certain server status.
