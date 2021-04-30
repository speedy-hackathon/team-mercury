export const restartGameRequest = async () => {
    console.log('log')
    const responce = await fetch('https://localhost:5001/api/restart');
    return responce;
}
