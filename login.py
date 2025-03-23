from requests import post, RequestException
def login(username, password) -> tuple[str, str]:

    url = 'https://easy-plu.knowledge-hero.com/api/plu/login'

    payload = {
        'name': username,
        'password': password
    }

    response = post(url, json=payload)
    response.raise_for_status()
    data = response.json()
    
    return data['api_token'], data['user']['id']