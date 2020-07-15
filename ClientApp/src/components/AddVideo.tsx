import React, { useState } from 'react';
import { Form, FormGroup, Label, Input, Button } from 'reactstrap';
import Axios from 'axios';
import swal from 'sweetalert';

export function AddVideo() {
  const [videoUrl, setVideoUrl] = useState<string>('');
  const [channelUrl, setChannelUrl] = useState<string>('');
  const [language, setLanguage] = useState<string>('DE');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await Axios.post('https://localhost:5001/api/addvideo/', { videoId: videoUrl, channelId: channelUrl }, { headers: { 'Content-Type': 'application/json' } });
      swal({ text: 'Video submitted successfully', timer: 5000 });
    } catch (error) {
      swal({ text: 'Some error happened', timer: 5000 });
    }
  }
  return (
    <Form>
      <FormGroup>
        <Label><h1>Video Id:</h1></Label>
        <Input type="text" placeholder="https://www.youtube.com/watch?v=cfPh6oLBwn4" value={videoUrl} onChange={(e) => setVideoUrl(e.target.value)} />
        <Label><h1>Channel Id:</h1></Label>
        <Input type="text" placeholder="https://www.youtube.com/channel/UCuhEoi_0aIMKavKnNF_MY3g" value={channelUrl} onChange={(e) => setChannelUrl(e.target.value)} />
        <Label><h1>Language:</h1></Label>
        <Input type="text" placeholder="EN" value={language} onChange={(e) => setLanguage(e.target.value)} />
        <Button type="submit" color="primary"
          onClick={handleSubmit}>Save</Button>{' '}
      </FormGroup>
    </Form>
  );
}
