To test a POST request in Chrome Console

fetch('https://server-dev.rukkou.com:5003/make-call', {
  method: 'POST',
  body: JSON.stringify({
    title: 'foo',
    body: 'bar',
    userId: 1
  }),
  headers: {
    'Content-type': 'application/json; charset=UTF-8'
  }
})
.then(response => response.json())
.then(data => console.log(data))
;


To test a GET

fetch('https://server-dev.rukkou.com:5003/text')
  .then(response => response.text())
  .then(data => console.log(data));
