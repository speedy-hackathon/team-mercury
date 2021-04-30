export const restartGameRequest = async () => {
    const responce = await fetch('https://localhost:5001/api/action');
    return responce;
}
