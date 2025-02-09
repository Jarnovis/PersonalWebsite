export const pull = async (backendUrl) =>
{
    try {
        const response = await fetch(backendUrl, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include'
        })

        const data = await response.json();
        console.log(response);

        if (!response.ok) {
            throw new Error(data);
        }

        return data;
    }
    catch (error) {
        console.error(error);
        return null;
    }
}