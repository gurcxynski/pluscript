from requests import post

def login(username, password):  
    login_url = 'https://easy-plu.knowledge-hero.com/api/plu/login'

    payload = {
        'name': username,
        'password': password
    }

    headers = {
        'Content-Type': 'application/json'
    }

    response = post(login_url, json=payload, headers=headers).json()
    
    auth_token = response['api_token']
    user_id = response['user']['id']

    return auth_token, user_id