#!/usr/bin/env bash

export K6_WEB_DASHBOARD=true
export K6_WEB_DASHBOARD_PERIOD=1s
export K6_WEB_DASHBOARD_EXPORT=report.html

./k6.exe run script.js
