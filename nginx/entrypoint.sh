#!/bin/sh
echo "NGINX dev entrypoint is running"
crontab scheduler.txt
crontab -l

nginx -g 'daemon off;'