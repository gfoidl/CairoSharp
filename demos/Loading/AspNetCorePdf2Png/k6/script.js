import { group, check } from "k6";
import http             from "k6/http";
import { expect }       from 'https://jslib.k6.io/k6-testing/0.6.1/index.js';
//-----------------------------------------------------------------------------
export default function () {
    let res;

    group("default", function () {
        const url = "http://localhost:5097";
        res       = http.get(url);
    });

    group("stream", function () {
        const url = "http://localhost:5097/stream";
        res       = http.get(url);
    });

    expect(res.headers["Content-Type"]).toBeDefined();
    expect(res.headers["Content-Type"]).toEqual("image/png");

    check(res, {
        "is HTTP 200"      : r => r.status === 200,
        "body is not empty": r => r.body.length > 0
    });
}
//-----------------------------------------------------------------------------
export const options = {
    thresholds: {
        http_req_failed  : ["rate < 0.001"],
        http_req_duration: ["p(95) < 1000", "med < 600"]   // ms
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
