from requests import post, put

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

    response = post(url, json=payload, headers=headers)

    return response.json()["data"]["session_id"]

def get_execution_items(token, session_id):
    url = f"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/execution-items"

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    response = post(url, headers=headers)
    
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

    response = post(url, headers=headers)
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

    put(url, json=payload, headers=headers)

def get_result(token, session_id, user_id):
    url = f"https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/result"

    headers = {
        'accept': 'application/json',
        'accept-encoding': 'gzip, deflate, br, zstd',
        'authorization': f'Bearer {token}',
        'content-type': 'application/json',
    }

    payload = { 'user_id': user_id }

    response = post(url, json=payload, headers=headers)
    result = response.json()["data"]["result"]["total_user_points"]
    max_result = response.json()["data"]["result"]["max_points"]
    return (result, max_result)