import requests

def login():
    login = input("Podaj login: ")
    password = input("Podaj hasło: ")   
    login_url = 'https://easy-plu.knowledge-hero.com/api/plu/login'
    payload = {
        'name': login,
        'password': password
    }
    headers = {
        'Content-Type': 'application/json'
    }

    login_response = requests.post(login_url, json=payload, headers=headers).json()
    token = login_response['api_token']
    id = login_response['user']['id']
    return token, id

def get_plu_number(token, phrase):
    url = 'https://easy-plu.knowledge-hero.com/api/plu/product/search'

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    payload = {
        "page": 1,
        "perPage": 1,
        "search": phrase,
    }

    response = requests.post(url, json=payload, headers=headers)
    data = response.json()
    return data["data"]["products"]["data"][0]["plu_number"]

def create_new_session(token, user_id):
    url = "https://easy-plu.knowledge-hero.com/api/plu/plu-learn/create-new-session"

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    payload = {
        "product_category_id":None,
        "count_selection":20,
        "user_id":user_id,
        "language_id":20,
        "execution_type":3,
        "execution_subtype":0,
        "ean_active":False,
        "top_article_active":False,
        "plu_current_count":1,
        "attribute_group_id":None,
        "new_plu":None}

    response = requests.post(url, json=payload, headers=headers)

    data = response.json()
    return data["data"]["session_id"]

def get_execution_items(session_id):
    url = f"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/execution-items"

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    response = requests.post(url, headers=headers)
    
    data = response.json()

    ret = []

    for item in data["data"]["items"]:
        pluobj = item["pluNumber"]
        task_id = item["id"]
        title = pluobj["title"]
        if title == None:
            title = next(iter(pluobj["translations"].values()))["title"]
        id = pluobj["id"]
        ret.append([title, id, task_id])

    return ret

def start_execution(token, session_id):
    url = f"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/start-execution"

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    response = requests.post(url, headers=headers)
    return response.json()

def send_answer(token, task_id, plu_number, plu_id):
    url = f"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{task_id}/update"

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    payload = {
        "execution_type":3,
        "given_plu_number":plu_number,
        "plu_number_id":plu_id,
        "answer": {"correct":True}
    }

    requests.put(url, json=payload, headers=headers)

def get_result(token, session_id, user_id):
    url = f"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/result"

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    payload = {"user_id":user_id}

    response = requests.post(url, json=payload, headers=headers)
    result = response.json()["data"]["result"]["total_user_points"]
    max_result = response.json()["data"]["result"]["max_points"]
    return (result, max_result)


token, user_id = login()

while True:

    session_id = create_new_session(token, user_id)

    items = get_execution_items(session_id)

    start_execution(token, session_id)

    for item in items:
        print(item)
        plu = get_plu_number(token, item[0])
        print(plu)
        send_answer(token, item[2], plu, item[1])

    result = get_result(token, session_id, user_id)
    print(result)