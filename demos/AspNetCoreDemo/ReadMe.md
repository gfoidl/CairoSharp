# AspNetCoreDemo

## Showcase

This demo has a simple minimal api endpoint, that uses _cairo_ to render a SVG and write it to the output without the need of temporary files. This is done by either using the streaming API or registering a callback at surface construction.

Endpoints:
* http://localhost:5097 for gradient SVG demo
* http://localhost:5097?showtext=true prints additionally a text onto the surface

## k6 Load tests

### Download

Download k6 from [GitHub Releases](https://github.com/grafana/k6/releases).

### Running

#### General

See [Test for performance](https://grafana.com/docs/k6/latest/examples/get-started-with-k6/test-for-performance/) for information about the test-types.

In [How to use options](https://grafana.com/docs/k6/latest/using-k6/k6-options/how-to/) the options to run the tests are described.

#### Docker

```bash
cat script.js | docker run -i loadimpact/k6 run --vus 10 --duration 30s -
```

#### CLI

```bash
k6 run script.js --vus 10 --duration 30s
```

### Visualize

#### Dashboard

[Web Dashboard](https://grafana.com/docs/k6/latest/results-output/web-dashboard/) shows how to.

In short:
```bash
K6_WEB_DASHBOARD=true k6 run script.js
```

By default, the web dashboard is available on localhost port `5665`, so on http://localhost:5665.

To export a HTML report run
```bash
K6_WEB_DASHBOARD=true K6_WEB_DASHBOARD_EXPORT=report.html k6 run script.js
```

`K6_WEB_DASHBOARD_PERIOD` (defaults to `10s`) is the update interval of the web dashboard.

Further options can be found in [Dashboard options](https://grafana.com/docs/k6/latest/results-output/web-dashboard/#dashboard-options).

```bash
K6_WEB_DASHBOARD=true K6_WEB_DASHBOARD_EXPORT=report.html K6_WEB_DASHBOARD_PERIOD=1s k6 run script.js
```
