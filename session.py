from requests import post, put

def createNewSession(token, user_id, item_count) -> str:
    url = 'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/create-new-session'

    headers = { 'authorization': f'Bearer {token}' }

    payload = {
        'product_category_id': None,
        'count_selection': item_count,
        'user_id': user_id,
        'language_id': 20,
        'execution_type': 3,
        'execution_subtype': 0,
        'ean_active': False,
        'top_article_active': False,
        'plu_current_count': 1,
        'attribute_group_id': None,
        'new_plu': None
    }
    
    response = post(url, json=payload, headers=headers)
    response.raise_for_status()
    data = response.json()

    return data['data']['session_id']

def getExecutionItems(token, session_id) -> list:
    url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/execution-items'

    headers = { 'authorization': f'Bearer {token}' }

    response = post(url, headers=headers)
    response.raise_for_status()
    data = response.json()

    ret = []
    for item in data['data']['items']:
        pluobj = item['pluNumber']
        title = pluobj['title'] or next(iter(pluobj['translations'].values()))['title'] 
        ret.append([title, pluobj['id'], item['id']])
    return ret

def startExecution(token, session_id) -> None:
    url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/start-execution'

    headers = { 'authorization': f'Bearer {token}' }

    response = post(url, headers=headers)
    response.raise_for_status()

def sendAnswer(token, task_id, plu_number, plu_id) -> None:
    url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{task_id}/update'

    headers = { 'authorization': f'Bearer {token}' }

    payload = {
        'execution_type': 3,
        'given_plu_number': plu_number,
        'plu_number_id': plu_id,
        'answer': {'correct': True}
    }

    response = put(url, json=payload, headers=headers)
    response.raise_for_status()

def getResult(token, session_id, user_id) -> tuple[int, int]:
    url = f'https://easy-plu.knowledge-hero.com/api/plu/plu-learn/{session_id}/result'

    headers = { 'authorization': f'Bearer {token}' }

    payload = { 'user_id': user_id }

    response = post(url, json=payload, headers=headers)
    response.raise_for_status()
    data = response.json()

    result = data['data']['result']['total_user_points']
    max_result = data['data']['result']['max_points']

    return result, max_result