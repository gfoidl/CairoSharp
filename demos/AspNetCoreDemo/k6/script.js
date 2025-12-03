import { group, check } from "k6";
import http             from "k6/http";
import { Counter }      from "k6/metrics";
import { expect }       from 'https://jslib.k6.io/k6-testing/0.6.1/index.js';
//-----------------------------------------------------------------------------
export const defaultDataReceived = new Counter("default_data_recv");
export const textDataReceived    = new Counter("text_data_recv");
//-----------------------------------------------------------------------------
function sizeOfHeaders(headers) {
    return Object.keys(headers).reduce((sum, key) => sum + key.length + headers[key].length, 0);
}
//-----------------------------------------------------------------------------
export default function () {
    let res;

    group("default", function () {
        const url = "http://localhost:5097";
        res       = http.get(url);

        defaultDataReceived.add(sizeOfHeaders(res.headers) + res.body.length);
    });

    group("text", function () {
        const url = "http://localhost:5097?showtext=true";
        res       = http.get(url);

        textDataReceived.add(sizeOfHeaders(res.headers) + res.body.length);
    });

    expect(res.headers["Content-Type"]).toBeDefined();
    expect(res.headers["Content-Type"]).toEqual("image/svg+xml");

    const contentIsOk = res.body.includes('<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="500" height="500" viewBox="0 0 500 500">');
    expect(contentIsOk).toBeTruthy();

    check(res, {
        "is HTTP 200"      : r => r.status === 200,
        "body is not empty": r => r.body.length > 0
    });
}
//-----------------------------------------------------------------------------
export const options = {
    thresholds: {
        http_req_failed  : ["rate < 0.001"],
        http_req_duration: ["p(95) < 75", "med < 20"]   // ms
    },
    scenarios: {
        average_load: {
            executor: "ramping-vus",
            stages  : [
                { duration:  "5s", target: 10 },
                { duration: "10s", target: 10 },
                { duration:  "5s", target:  0 }
            ]
        }
    }
};
