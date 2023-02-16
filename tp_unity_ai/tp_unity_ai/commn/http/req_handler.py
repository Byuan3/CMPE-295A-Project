import io
import cv2
from http.server import BaseHTTPRequestHandler
import cgi

import numpy as np


class RequestHandler(BaseHTTPRequestHandler):
    def do_POST(self):
        ctype, pdict = cgi.parse_header(self.headers.get('Content-Type'))
        if ctype == 'multipart/form-data':
            pdict['boundary'] = bytes(pdict['boundary'], "utf-8")
            fields = cgi.parse_multipart(self.rfile, pdict)
            img = fields.get('image')
            msg = fields.get('msg')
            if img is not None:
                img_file = io.BytesIO(img[0])
                nparr = np.frombuffer(img_file.getvalue(), np.uint8)
                img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
                # Process the image and msg as needed
                cv2.imshow("Received Image", img)
                cv2.waitKey(1)
                if msg is not None:
                    print(msg[0])
                # Set response
                self.send_response(200)
                self.send_header('Content-type', 'text/plain')
                self.end_headers()
                self.wfile.write(b'Success')
                return
        self.send_response(400)
        self.end_headers()
        