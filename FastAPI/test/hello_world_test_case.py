import requests

response = requests.get('http://127.0.0.1:7000/debug/hello_world')
print(response.json())