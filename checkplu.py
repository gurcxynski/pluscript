from requests import post, RequestException

def getPluNumber(token, phrase) -> str:
    url = 'https://easy-plu.knowledge-hero.com/api/plu/product/search'

    headers = { 'authorization': f'Bearer {token}' }

    payload = { 'search': phrase }

    try:
        response = post(url, json=payload, headers=headers)
        response.raise_for_status()
        data = response.json()

        list = data['data']['products']['data']

        if len(list) == 0:
            raise ValueError(f'No products found for {phrase}')
        
        best = list[0]
        plu = best['plu_number']

        return plu

    except RequestException as e:
        print(f'Web request failed: {e}')
    except ValueError as e:
        print(f'JSON decoding failed: {e}')
